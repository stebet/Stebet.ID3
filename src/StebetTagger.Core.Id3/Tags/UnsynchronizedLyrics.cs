using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace StebetTagger.Core.Id3.Tags
{
    public class UnsynchronizedLyrics : Frame
    {
        public string Language { get; set; }
        public string Description { get; set; }
        public string Text { get; set; }

        internal override byte[] GetContentBytes(TagVersion version)
        {
            List<byte> bytes = new List<byte>();
            switch (version)
            {
                case TagVersion.V23:
                    bytes.Add(0x00);
                    bytes.AddRange(Encoding.Default.GetBytes("eng"));
                    if (!string.IsNullOrEmpty(Description))
                    {
                        bytes.AddRange(Encoding.Default.GetBytes(Description));
                    }
                    bytes.Add(0x00);
                    if (!string.IsNullOrEmpty(Text))
                    {
                        bytes.AddRange(Encoding.Default.GetBytes(Text));
                    }
                    bytes.Add(0x00);
                    break;
                case TagVersion.V24:
                default:
                    throw new ArgumentException("Version " + version.ToString() + " is not supported for this frame!", nameof(version));
            }
            return bytes.ToArray();
        }

        internal override string GetTagId(TagVersion version)
        {
            switch (version)
            {
                case TagVersion.V23:
                    return "USLT";
                case TagVersion.V24:
                default:
                    throw new ArgumentException($"Version {version} is not supported for this frame!", nameof(version));
            }
        }

        public override async Task FromStreamAsync(Stream stream, int tagLength, TagVersion version)
        {
            if (tagLength > 6)
            {
                long streamStart = stream.Position;
                int encoding = stream.ReadByte();
                var languageBytes = new byte[3];
                await stream.ReadAsync(languageBytes, 0, languageBytes.Length).ConfigureAwait(false);
                Language = Encoding.Default.GetString(languageBytes);
                switch (encoding)
                {
                    case 0x00:
                        Description = await stream.ReadAnsiString(streamStart + tagLength).ConfigureAwait(false);
                        Text = await stream.ReadAnsiString(streamStart + tagLength).ConfigureAwait(false);
                        break;
                    case 0x01:
                        Description = await stream.ReadUnicodeStringAsync(streamStart + tagLength).ConfigureAwait(false);
                        Text = await stream.ReadUnicodeStringAsync(streamStart + tagLength).ConfigureAwait(false);
                        break;
                    default:
                        throw new ArgumentException("Frame has an invalid encoding marker", nameof(stream));
                }
            }
        }

        public UnsynchronizedLyrics()
        {
        }
    }
}
