using System.Buffers;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace StebetTagger.Core.Id3
{
    public static class StreamExtensions
    {
        public static async Task<string> ReadAnsiString(this Stream stream, long limit)
        {
            long streamStart = stream.Position;
            while (stream.Position < limit && !(stream.ReadByte() == 0x00))
            {
            }

            stream.Seek(-1, SeekOrigin.Current);
            int size = (int)(stream.Position - streamStart);
            var stringBytes = ArrayPool<byte>.Shared.Rent(size);
            stream.Seek(streamStart, SeekOrigin.Begin);
            await stream.ReadAsync(stringBytes, 0, size).ConfigureAwait(false);
            stream.Seek(1, SeekOrigin.Current);
            string results = Encoding.Default.GetString(stringBytes, 0, size);
            ArrayPool<byte>.Shared.Return(stringBytes);
            return results;
        }

        public static async Task<string> ReadUnicodeStringAsync(this Stream stream, long limit)
        {
            var encoding = (stream.ReadByte() == 0xFF && stream.ReadByte() == 0xFE) ? Encoding.Unicode : Encoding.BigEndianUnicode;
            long streamStart = stream.Position;
            while (stream.Position < limit && !(stream.ReadByte() == 0x00 && stream.ReadByte() == 0x00))
            {
            }
            stream.Seek(-1, SeekOrigin.Current);
            int size = (int)(stream.Position - streamStart);
            var stringBytes = ArrayPool<byte>.Shared.Rent(size);
            stream.Seek(streamStart, SeekOrigin.Begin);
            await stream.ReadAsync(stringBytes, 0, size).ConfigureAwait(false);
            stream.Seek(2, SeekOrigin.Current);
            string results = encoding.GetString(stringBytes, 0, size);
            ArrayPool<byte>.Shared.Return(stringBytes);
            return results;
        }
    }
}
