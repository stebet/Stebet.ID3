using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StebetTagger.Core.Id3
{
    public static class StreamExtensions
    {
        public static string ReadAnsiString(this Stream stream, long limit)
        {
            var byteList = new List<byte>();
            long streamStart = stream.Position;
            while (stream.Position < limit)
            {
                var lastByte = stream.ReadByte();
                if(lastByte != 0x00)
                {
                    byteList.Add((byte)lastByte);
                }
                else
                { 
                    break;
                }
            }

            return Encoding.Default.GetString(byteList.ToArray());
        }

        public static async Task<string> ReadUnicodeStringAsync(this Stream stream, long limit)
        {
            var encoding = (stream.ReadByte() == 0xFF && stream.ReadByte() == 0xFE) ? Encoding.Unicode : Encoding.BigEndianUnicode;
            long streamStart = stream.Position;
            while (stream.Position < limit && !(stream.ReadByte() == 0x00 && stream.ReadByte() == 0x00))
            {
            }
            stream.Seek(-1, SeekOrigin.Current);
            var stringBytes = new byte[stream.Position - streamStart];
            stream.Seek(streamStart, SeekOrigin.Begin);
            await stream.ReadAsync(stringBytes, 0, stringBytes.Length).ConfigureAwait(false);
            stream.ReadByte();
            stream.ReadByte();
            return encoding.GetString(stringBytes);
        }
    }
}
