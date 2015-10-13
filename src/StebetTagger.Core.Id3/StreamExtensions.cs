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
        public static async Task<string> ReadAnsiStringAsync(this Stream stream, long limit)
        {
            long streamStart = stream.Position;
            while (stream.Position < limit && stream.ReadByte() != 0x00)
            {
            }
            stream.Seek(-1, SeekOrigin.Current);
            try
            {
                var stringBytes = new byte[stream.Position - streamStart];
                stream.Seek(streamStart, SeekOrigin.Begin);
                await stream.ReadAsync(stringBytes, 0, stringBytes.Length).ConfigureAwait(false);
                stream.ReadByte();
                return Encoding.Default.GetString(stringBytes);
            }
            catch (Exception ex)
            {
                string test = "";
                throw;
            }
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
