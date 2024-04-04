using Steamworks;
using System.IO;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerVoiceChat : NetworkBehaviour, IStreamedAudio
{
    private NetworkVariable<FixedString4096Bytes> _voiceData = new(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private AudioSource _source;

    private MemoryStream _output;
    private MemoryStream _stream;
    private MemoryStream _input;

    private int _optimalRate;
    public int ClipBufferSize { get; private set; }
    public float[] ClipBuffer { get; private set; }

    private int _playbackBuffer;
    private int _dataPosition;
    private int _dataReceived;

    private int _compressedWritten;

    public bool Listen = false;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
        _voiceData.OnValueChanged += OnVoiceDataChanged;
    }

    private void Start()
    {
        _optimalRate = (int)SteamUser.OptimalSampleRate;

        ClipBufferSize = _optimalRate * 5;
        ClipBuffer = new float[ClipBufferSize];

        _stream = new MemoryStream();
        _output = new MemoryStream();
        _input = new MemoryStream();

        _source.clip = AudioClip.Create("VoiceData", 256, 1, _optimalRate, true, OnAudioRead, null);
        _source.loop = true;
        _source.Play();
    }

    private void Update()
    {
        if (gameObject.GetComponentInParent<NetworkObject>().IsLocalPlayer)
        {
            if (SteamUser.HasVoiceData)
            {
                var stream = new MemoryStream();
                _compressedWritten = SteamUser.ReadVoiceData(stream);
                stream.Position = 0;

                string streamString = null;
                foreach (byte b in stream.GetBuffer())
                    streamString += b.ToString() + ",";

                streamString = streamString.Remove(streamString.Length - 1, 1);
                var fixedStreamString = new FixedString4096Bytes(streamString);

                _voiceData.Value = fixedStreamString;
            }
            if (_playbackBuffer == 0)
                ClipBuffer = new float[ClipBufferSize];
        }
    }

    public void OnVoiceDataChanged(FixedString4096Bytes previous, FixedString4096Bytes current)
    {
        if (IsOwner && !Listen) return;

        string[] strs = current.ToString().Split(",");
        byte[] buffer = new byte[strs.Length];
        for (int i = 0; i < strs.Length; i++)
            buffer[i] = byte.Parse(strs[i]);

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
                _dataPosition = (_dataPosition + 1) % ClipBufferSize;

                data[i] = ClipBuffer[_dataPosition];

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
            ClipBuffer[_dataReceived] = converted;

            // buffer loop
            _dataReceived = (_dataReceived + 1) % ClipBufferSize;

            _playbackBuffer++;
        }
    }
}