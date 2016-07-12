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
                stringBytes[len] = (byte)stream.ReadByte();
                if (stringBytes[len] == 0x00)
                {
                    break;
                }

                len++;
            }

            string results = Encoding.Default.GetString(stringBytes, 0, len);
            ArrayPool<byte>.Shared.Return(stringBytes);
            return results;
        }

        public static async Task<string> ReadUnicodeStringAsync(this Stream stream, long limit)
        {
            byte a = (byte)stream.ReadByte();
            byte b = (byte)stream.ReadByte();

            var encoding = (a == 0xFF && b == 0xFE) ? Encoding.Unicode : Encoding.BigEndianUnicode;
            var stringBytes = ArrayPool<byte>.Shared.Rent((int)(limit - stream.Position));

            int len = 0;
            while (stream.Position < limit)
            {
                stringBytes[len] = (byte)stream.ReadByte();
                stringBytes[len + 1] = (byte)stream.ReadByte();
                if (stringBytes[len + 1] == 0x00 && stringBytes[len] == 0x00)
                {
                    break;
                }

                len += 2;
            }

            string results = encoding.GetString(stringBytes, 0, len);
            ArrayPool<byte>.Shared.Return(stringBytes);
            return results;
        }
    }
}
