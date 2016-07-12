using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace StebetTagger.Core.Id3.Tags
{
    public abstract class TextFrame : Frame
    {
        public string Text { get; set; }
        public Encoding Encoding { get; set; }

        internal override byte[] GetContentBytes(TagVersion version)
        {
            switch (version)
            {
                case TagVersion.V23:
                    var bytes = new List<byte>();
                    if (Encoding == Encoding.GetEncoding("ISO-8859-1"))
                    {
                        bytes.Add(0x00);
                        if (!string.IsNullOrEmpty(Text))
                        {
                            bytes.AddRange(Encoding.GetBytes(Text));
                        }
                        bytes.Add(0x00);
                    }
                    else if (Encoding == Encoding.Unicode || Encoding == Encoding.BigEndianUnicode)
                    {
                        bytes.Add(0x01);
                        if (!string.IsNullOrEmpty(Text))
                        {
                            bytes.AddRange(Encoding.GetPreamble());
                            bytes.AddRange(Encoding.GetBytes(Text));
                        }
                        bytes.Add(0x00);
                        bytes.Add(0x00);
                    }
                    else
                    {
                        throw new InvalidOperationException("Unknown encoding!");
                    }

                    return bytes.ToArray();
                default:
                    throw new NotImplementedException("Writing " + version.ToString() + " has not been implemented!");
            }
        }

        public override async Task FromStreamAsync(Stream stream, uint tagLength, TagVersion version)
        {
            switch (version)
            {
                case TagVersion.V23:
                    if (tagLength > 1)
                    {
                        long startPosition = stream.Position;
                        int peek = stream.ReadByte();
                        if (peek == 0x00)
                        {
                            Text = await stream.ReadAnsiString(startPosition + tagLength).ConfigureAwait(false);
                        }
                        else if (peek == 0x01)
                        {
                            Text = await stream.ReadUnicodeStringAsync(startPosition + tagLength).ConfigureAwait(false);
                        }
                        else
                        {
                            throw new ArgumentException("frame does not contain valid text data!", "frame");
                        }
                    }
                    else
                    {
                        throw new ArgumentException("frame does not contain valid text data!", "frame");
                    }
                    break;
                default:
                    throw new NotImplementedException("Reading " + version.ToString() + " has not been implemented!");
            }
        }

        public override string ToString() => Text;
    }
}
