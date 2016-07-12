using System.Buffers;
using System.IO;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace StebetTagger.Core.Id3
{
    public static class StreamExtensions
    {
        private static readonly Encoding ISO = Encoding.GetEncoding("ISO-8859-1");
        private static readonly Vector<byte> byteZero = Vector<byte>.Zero;
        private static readonly int stride = Vector<byte>.Count;

        public static async Task<string> ReadAnsiString(this Stream stream, long limit)
        {
            int size = (int)(limit - stream.Position);
            var originalPosition = stream.Position;
            var stringBytes = new byte[size];
            await stream.ReadAsync(stringBytes, 0, size).ConfigureAwait(false);
            int len = FindZeroIndexAnsi(stringBytes);

            if (len == -1)
                len = size;

            if (len != size)
            {
                stream.Seek(originalPosition + len + 1, SeekOrigin.Begin);
            }

            string results = ISO.GetString(stringBytes, 0, len);
            //ArrayPool<byte>.Shared.Return(stringBytes);
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

        public static int FindZeroIndexAnsi(byte[] array)
        {
            int numIters = array.Length / stride;
            for (int i = 0; i < numIters; i++)
            {
                var byte0Equals = Vector.Equals(new Vector<byte>(array, i * stride), byteZero);
                if (byte0Equals.Equals(byteZero))
                {
                    continue;
                }

                return (i * stride) + FindLongIndex(ref byte0Equals);
            }

            if (numIters * stride < array.Length)
            {
                for (int i = numIters * stride; i < array.Length; i++)
                {
                    if (array[i] == 0x00)
                        return i;
                }
            }

            return -1;
        }

        public static int FindLongIndex(ref Vector<byte> bytes)
        {
            var vector64 = Vector.AsVectorInt64(bytes);
            for (int i = 0; i < Vector<long>.Count; i++)
            {
                var longValue = vector64[i];
                if (longValue == 0)
                    continue;

                return (i << 3) +
                    ((longValue & 0x00000000ffffffff) > 0 ?
                      (longValue & 0x000000000000ffff) > 0 ?
                        (longValue & 0x00000000000000ff) > 0 ? 0 : 1
                        :
                        (longValue & 0x0000000000ff0000) > 0 ? 2 : 3
                      :
                      (longValue & 0x0000ffff00000000) > 0 ?
                        (longValue & 0x000000ff00000000) > 0 ? 4 : 5
                        :
                        (longValue & 0x00ff000000000000) > 0 ? 6 : 7);
            }

            return 0;
        }
    }
}
