using NAudio.Wave;

namespace EscapeFromDungeon.Services
{
    public class MusicPlayer
    {
        private WaveOutEvent? outputDevice;
        private WaveStream? stream;

        public void PlayLoop(UnmanagedMemoryStream resourceStream)
        {
            Stop();

            // UnmanagedMemoryStream → MemoryStream に変換
            var memoryStream = new MemoryStream();
            resourceStream.CopyTo(memoryStream);
            memoryStream.Position = 0;

            var reader = new Mp3FileReader(memoryStream);
            stream = new LoopStream(reader);

            outputDevice = new WaveOutEvent();
            outputDevice.Init(stream);
            outputDevice.Play();
        }

        public void PlayOnce(UnmanagedMemoryStream resourceStream)
        {
            Stop();

            var memoryStream = new MemoryStream();
            resourceStream.CopyTo(memoryStream);
            memoryStream.Position = 0;

            stream = new Mp3FileReader(memoryStream);

            outputDevice = new WaveOutEvent();
            outputDevice.Init(stream);
            outputDevice.Play();
        }

        public void Stop()
        {
            outputDevice?.Stop();
            outputDevice?.Dispose();
            stream?.Dispose();
        }
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
