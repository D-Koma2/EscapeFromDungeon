using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace EscapeFromDungeon.Services
{
    public class MusicPlayer : IDisposable
    {
        private const float _volumeBgm = 0.6f;
        private const float _volumeSE = 0.8f;

        private WaveOutEvent? _outputDevice;
        private WaveStream? _stream;

        public void PlayLoop(UnmanagedMemoryStream resourceStream)
        {
            Stop();

            // UnmanagedMemoryStream → MemoryStream に変換
            var memoryStream = new MemoryStream();
            resourceStream.CopyTo(memoryStream);
            memoryStream.Position = 0;

            var reader = new Mp3FileReader(memoryStream);
            var volumeProvider = new VolumeSampleProvider(reader.ToSampleProvider());
            volumeProvider.Volume = _volumeBgm;

            _stream = new LoopStream(reader);

            _outputDevice = new WaveOutEvent();
            _outputDevice.Init(volumeProvider);
            _outputDevice.Play();
        }

        public void PlayOnce(UnmanagedMemoryStream resourceStream)
        {
            Stop();

            var memoryStream = new MemoryStream();
            resourceStream.CopyTo(memoryStream);
            memoryStream.Position = 0;

            _stream = new Mp3FileReader(memoryStream);
            var volumeProvider = new VolumeSampleProvider(_stream.ToSampleProvider());
            volumeProvider.Volume = _volumeSE;

            _outputDevice = new WaveOutEvent();
            _outputDevice.Init(volumeProvider);
            _outputDevice.Play();
        }

        public void Stop()
        {
            _outputDevice?.Stop();
            _outputDevice?.Dispose();
            _stream?.Dispose();
        }

        public void Dispose() => Stop();
    }

    public class LoopStream : WaveStream
    {
        private readonly WaveStream sourceStream;

        public LoopStream(WaveStream sourceStream)
        {
            this.sourceStream = sourceStream;
        }

        public override WaveFormat WaveFormat => sourceStream.WaveFormat;
        public override long Length => long.MaxValue;
        public override long Position
        {
            get => sourceStream.Position;
            set => sourceStream.Position = value;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int read = sourceStream.Read(buffer, offset, count);
            if (read == 0)
            {
                sourceStream.Position = 0;
                read = sourceStream.Read(buffer, offset, count);
            }
            return read;
        }
    }
}
