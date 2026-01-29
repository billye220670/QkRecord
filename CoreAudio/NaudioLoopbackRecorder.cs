using System;
using System.IO;
using System.Threading;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.Lame;

namespace AudioRecorder.CoreAudio
{
    public class NaudioLoopbackRecorder : IDisposable
    {
        private readonly string _filePath;
        private readonly int _sampleRate;
        private readonly int _channels;
        private readonly int _bitsPerSample;
        private readonly string _audioFormat;

        private WasapiLoopbackCapture? _capture;
        private object? _writer;
        private bool _isRecording;
        private long _dataSize;
        private readonly object _lockObj = new object();

        public NaudioLoopbackRecorder(string filePath, string audioFormat = "wav", int sampleRate = 44100, int channels = 2, int bitsPerSample = 16)
        {
            _filePath = filePath;
            _sampleRate = sampleRate;
            _channels = channels;
            _bitsPerSample = bitsPerSample;
            _audioFormat = audioFormat.ToLower();
        }

        ~NaudioLoopbackRecorder()
        {
            Dispose(false);
        }

        public bool Start()
        {
            lock (_lockObj)
            {
                if (_isRecording)
                    return true;

                try
                {
                    _capture = new WasapiLoopbackCapture();

                    _capture.WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(_sampleRate, _channels);

                    string? directory = Path.GetDirectoryName(_filePath);
                    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    if (_audioFormat == "mp3")
                    {
                        _writer = new LameMP3FileWriter(_filePath, _capture.WaveFormat, 128);
                    }
                    else
                    {
                        _writer = new WaveFileWriter(_filePath, _capture.WaveFormat);
                    }

                    _capture.DataAvailable += OnDataAvailable;
                    _capture.RecordingStopped += OnRecordingStopped;

                    _capture.StartRecording();

                    _isRecording = true;

                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Start error: {ex.Message}");
                    Console.WriteLine($"Stack: {ex.StackTrace}");
                    Cleanup();
                    return false;
                }
            }
        }

        private void OnDataAvailable(object? sender, WaveInEventArgs e)
        {
            if (!_isRecording || _writer == null)
                return;

            try
            {
                lock (_lockObj)
                {
                    if (_isRecording && _writer != null)
                    {
                        if (_writer is WaveFileWriter waveWriter)
                        {
                            waveWriter.Write(e.Buffer, 0, e.BytesRecorded);
                        }
                        else if (_writer is LameMP3FileWriter mp3Writer)
                        {
                            mp3Writer.Write(e.Buffer, 0, e.BytesRecorded);
                        }
                        _dataSize += e.BytesRecorded;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Data available error: {ex.Message}");
            }
        }

        private void OnRecordingStopped(object? sender, StoppedEventArgs e)
        {
            Console.WriteLine("Recording stopped");
        }

        public void Stop()
        {
            lock (_lockObj)
            {
                if (!_isRecording)
                    return;

                _isRecording = false;

                try
                {
                    _capture?.StopRecording();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Stop error: {ex.Message}");
                }

                Cleanup();
            }
        }

        private void Cleanup()
        {
            try
            {
                if (_capture != null)
                {
                    _capture.DataAvailable -= OnDataAvailable;
                    _capture.RecordingStopped -= OnRecordingStopped;
                    _capture.Dispose();
                    _capture = null;
                }

                if (_writer is IDisposable disposableWriter)
                {
                    disposableWriter.Dispose();
                    _writer = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cleanup error: {ex.Message}");
            }
        }

        public bool IsRecording => _isRecording;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            Stop();
        }
    }
}
