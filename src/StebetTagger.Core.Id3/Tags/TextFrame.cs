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
                        long textSize = 0;
                        int peek = stream.ReadByte();
                        if (peek == 0x00)
                        {
                            Encoding = Encoding.GetEncoding("ISO-8859-1");
                            while (stream.ReadByte() != 0x00 && (stream.Position - startPosition) < tagLength)
                            {
                            }

                            textSize = (stream.Position) - (startPosition + 1);

                            stream.Seek(startPosition + 1, SeekOrigin.Begin);

                            if (textSize > 0)
                            {
                                var textBytes = ArrayPool<byte>.Shared.Rent((int)textSize);
                                await stream.ReadAsync(textBytes, 0, textBytes.Length).ConfigureAwait(false);
                                Text = Encoding.GetString(textBytes);
                                ArrayPool<byte>.Shared.Return(textBytes);
                            }
                        }
                        else if (peek == 0x01)
                        {
                            int nextPeek = stream.ReadByte();
                            while (textSize < tagLength && (peek == 0x00 && nextPeek == 0x00) == false)
                            {
                                textSize += 2;
                            }

                            stream.Seek(-(stream.Position - startPosition), SeekOrigin.Current);

                            if (textSize > 0)
                            {
                                if (stream.ReadByte() == 0xFF && stream.ReadByte() == 0xFE)
                                {
                                    Encoding = Encoding.Unicode;
                                }
                                else
                                {
                                    Encoding = Encoding.BigEndianUnicode;
                                }

                                var textBytes = ArrayPool<byte>.Shared.Rent((int)textSize);
                                await stream.ReadAsync(textBytes, 0, textBytes.Length).ConfigureAwait(false);
                                Text = Encoding.GetString(textBytes);
                                ArrayPool<byte>.Shared.Return(textBytes);
                            }
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
