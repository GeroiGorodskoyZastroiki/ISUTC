using Steamworks;
using System.IO;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class VoiceChat : NetworkBehaviour
{
    public NetworkVariable<FixedString4096Bytes> voiceData = new NetworkVariable<FixedString4096Bytes>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private AudioSource source;

    private MemoryStream output;
    private MemoryStream stream;
    private MemoryStream input;

    private int optimalRate;
    private int clipBufferSize;
    private float[] clipBuffer;

    private int playbackBuffer;
    private int dataPosition;
    private int dataReceived;

    int compressedWritten;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        voiceData.OnValueChanged += OnVoiceDataChanged;
    }

    private void Start()
    {
        optimalRate = (int)SteamUser.OptimalSampleRate;

        clipBufferSize = optimalRate * 5;
        clipBuffer = new float[clipBufferSize];

        stream = new MemoryStream();
        output = new MemoryStream();
        input = new MemoryStream();

        source.clip = AudioClip.Create("VoiceData", (int)256, 1, (int)optimalRate, true, OnAudioRead, null);
        source.loop = true;
        source.Play();
    }

    private void Update()
    {
        if (gameObject.GetComponentInParent<NetworkObject>().IsLocalPlayer)
        {
            //SteamUser.VoiceRecord = Input.GetKey(KeyCode.V); //переписать
            if (SteamUser.HasVoiceData)
            {
                Debug.Log("voicechat");
                //int compressedWritten = SteamUser.ReadVoiceData(stream);
                //stream.Position = 0;

                var stream = new MemoryStream();
                compressedWritten = SteamUser.ReadVoiceData(stream);
                stream.Position = 0;

                string streamString = null;
                foreach (byte b in stream.GetBuffer())
                {
                    streamString += b.ToString() + ",";
                }
                streamString = streamString.Remove(streamString.Length - 1, 1);
                var fixedStreamString = new FixedString4096Bytes(streamString);

                voiceData.Value = fixedStreamString;
            }
        }
    }

    public void OnVoiceDataChanged(FixedString4096Bytes previous, FixedString4096Bytes current)
    {
        if (IsOwner) return;

        string[] strs = current.ToString().Split(",");
        byte[] buffer = new byte[strs.Length];
        for (int i = 0; i < strs.Length; i++)
        {
            buffer[i] = byte.Parse(strs[i]);
        }

        input.Write(buffer, 0, buffer.Length);
        input.Position = 0;

        int uncompressedWritten = SteamUser.DecompressVoice(input, buffer.Length, output);
        input.Position = 0;

        byte[] outputBuffer = output.GetBuffer();
        WriteToClip(outputBuffer, uncompressedWritten);
        output.Position = 0;
    }

    private void OnAudioRead(float[] data)
    {
        for (int i = 0; i < data.Length; ++i)
        {
            // start with silence
            data[i] = 0;

            // do I  have anything to play?
            if (playbackBuffer > 0)
            {
                // current data position playing
                dataPosition = (dataPosition + 1) % clipBufferSize;

                data[i] = clipBuffer[dataPosition];

                playbackBuffer--;
            }
        }
    }

    private void WriteToClip(byte[] uncompressed, int iSize)
    {
        for (int i = 0; i < iSize; i += 2)
        {
            // insert converted float to buffer
            float converted = (short)(uncompressed[i] | uncompressed[i + 1] << 8) / 32767.0f;
            clipBuffer[dataReceived] = converted;

            // buffer loop
            dataReceived = (dataReceived + 1) % clipBufferSize;

            playbackBuffer++;
        }
    }
}