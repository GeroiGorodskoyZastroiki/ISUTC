using Steamworks;
using System.IO;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerVoiceChat : NetworkBehaviour
{
    private NetworkVariable<FixedString4096Bytes> _voiceData = new(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private AudioSource _source;

    private MemoryStream _output;
    private MemoryStream _stream;
    private MemoryStream _input;

    private int _optimalRate;
    private int _clipBufferSize;
    private float[] _clipBuffer;

    private int _playbackBuffer;
    private int _dataPosition;
    private int _dataReceived;

    private int _compressedWritten;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
        _voiceData.OnValueChanged += OnVoiceDataChanged;
    }

    private void Start()
    {
        _optimalRate = (int)SteamUser.OptimalSampleRate;

        _clipBufferSize = _optimalRate * 5;
        _clipBuffer = new float[_clipBufferSize];

        _stream = new MemoryStream();
        _output = new MemoryStream();
        _input = new MemoryStream();

        _source.clip = AudioClip.Create("VoiceData", (int)256, 1, (int)_optimalRate, true, OnAudioRead, null);
        _source.loop = true;
        _source.Play();
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
                _compressedWritten = SteamUser.ReadVoiceData(stream);
                stream.Position = 0;

                string streamString = null;
                foreach (byte b in stream.GetBuffer())
                {
                    streamString += b.ToString() + ",";
                }
                streamString = streamString.Remove(streamString.Length - 1, 1);
                var fixedStreamString = new FixedString4096Bytes(streamString);

                _voiceData.Value = fixedStreamString;
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

        _input.Write(buffer, 0, buffer.Length);
        _input.Position = 0;

        int uncompressedWritten = SteamUser.DecompressVoice(_input, buffer.Length, _output);
        _input.Position = 0;

        byte[] outputBuffer = _output.GetBuffer();
        WriteToClip(outputBuffer, uncompressedWritten);
        _output.Position = 0;
    }

    private void OnAudioRead(float[] data)
    {
        for (int i = 0; i < data.Length; ++i)
        {
            // start with silence
            data[i] = 0;

            // do I  have anything to play?
            if (_playbackBuffer > 0)
            {
                // current data position playing
                _dataPosition = (_dataPosition + 1) % _clipBufferSize;

                data[i] = _clipBuffer[_dataPosition];

                _playbackBuffer--;
            }
        }
    }

    private void WriteToClip(byte[] uncompressed, int iSize)
    {
        for (int i = 0; i < iSize; i += 2)
        {
            // insert converted float to buffer
            float converted = (short)(uncompressed[i] | uncompressed[i + 1] << 8) / 32767.0f;
            _clipBuffer[_dataReceived] = converted;

            // buffer loop
            _dataReceived = (_dataReceived + 1) % _clipBufferSize;

            _playbackBuffer++;
        }
    }
}