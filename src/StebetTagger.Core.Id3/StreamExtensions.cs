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
            var stringBytes = ArrayPool<byte>.Shared.Rent((int)(limit - stream.Position));
            int len = 0;
            while (stream.Position < limit)
            {
                stringBytes[len++] = (byte)stream.ReadByte();
                if (stringBytes[len - 1] == 0x00)
                    break;
            }

            string results = Encoding.Default.GetString(stringBytes, 0, len - (stream.Position == limit ? 0 : 1));
            ArrayPool<byte>.Shared.Return(stringBytes);
            return results;
        }

        public static async Task<string> ReadUnicodeStringAsync(this Stream stream, long limit)
        {
            var encoding = (stream.ReadByte() == 0xFF && stream.ReadByte() == 0xFE) ? Encoding.Unicode : Encoding.BigEndianUnicode;

            var stringBytes = ArrayPool<byte>.Shared.Rent((int)(limit - stream.Position));
            int len = 0;
            do
            {
                stringBytes[len++] = (byte)stream.ReadByte();
                stringBytes[len++] = (byte)stream.ReadByte();
            }
            while (stringBytes[len - 1] != 0x00 && stringBytes[len - 2] != 0x00);

            string results = encoding.GetString(stringBytes, 0, len - 2);
            ArrayPool<byte>.Shared.Return(stringBytes);
            return results;
        }
    }
}
