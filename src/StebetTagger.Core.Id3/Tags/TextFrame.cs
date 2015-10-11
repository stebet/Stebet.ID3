using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
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
                    List<byte> bytes = new List<byte>();
                    if (this.Encoding == Encoding.GetEncoding("ISO-8859-1"))
                    {
                        bytes.Add(0x00);
                        if (!string.IsNullOrEmpty(Text))
                        {
                            bytes.AddRange(this.Encoding.GetBytes(Text));
                        }
                        bytes.Add(0x00);
                    }
                    else if (this.Encoding == Encoding.Unicode || this.Encoding == Encoding.BigEndianUnicode)
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

        public override async Task FromStream(Stream stream, int tagLength, TagVersion version)
        {
            switch (version)
            {
                case TagVersion.V23:
                    if (tagLength > 1)
                    {
                        long currentPosition = stream.Position;
                        int textStart = 1;
                        int textEnd = textStart;
                        int peek = stream.ReadByte(); 
                        if (peek == 0x00)
                        {
                            peek = stream.ReadByte();
                            this.Encoding = Encoding.GetEncoding("ISO-8859-1");
                            while (textEnd < tagLength && peek != 0x00)
                            {
                                textEnd++;
                                peek = stream.ReadByte();
                            }

                            stream.Seek(-(stream.Position - currentPosition) + 1, SeekOrigin.Current);

                            if (textEnd - textStart > 0)
                            {
                                byte[] textBytes = new byte[textEnd - textStart];
                                await stream.ReadAsync(textBytes, 0, textBytes.Length);
                                Text = this.Encoding.GetString(textBytes);
                            }
                        }
                        else if (peek == 0x01)
                        {
                            int nextPeek = stream.ReadByte();
                            while (textEnd < tagLength && (peek == 0x00 && nextPeek == 0x00) == false)
                            {
                                textEnd += 2;
                            }

                            stream.Seek(-(stream.Position - currentPosition), SeekOrigin.Current);

                            if (textEnd - textStart > 0)
                            {
                                if (stream.ReadByte() == 0xFF && stream.ReadByte() == 0xFE)
                                {
                                    this.Encoding = Encoding.Unicode;
                                }
                                else
                                {
                                    this.Encoding = Encoding.BigEndianUnicode;
                                }

                                byte[] textBytes = new byte[textEnd - textStart];
                                await stream.ReadAsync(textBytes, 0, textBytes.Length);
                                Text = this.Encoding.GetString(textBytes);
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

        public override void FromBytes(byte[] bytes, int tagLength, TagVersion version)
        {
            switch (version)
            {
                case TagVersion.V23:
                    if (tagLength > 1)
                    {
                        int textStart = 1;
                        int textEnd = 1;
                        int textLength = 0;
                        switch (bytes[0])
                        {
                            case 0x00:
                                this.Encoding = Encoding.GetEncoding("ISO-8859-1");
                                while (textEnd < tagLength && bytes[textEnd] != 0x00)
                                {
                                    textEnd++;
                                }

                                textLength = textEnd - textStart;
                                break;
                            case 0x01:
                                if (bytes[textStart] == 0xFF && bytes[textStart + 1] == 0xFE)
                                {
                                    this.Encoding = Encoding.Unicode;
                                }
                                else
                                {
                                    this.Encoding = Encoding.BigEndianUnicode;
                                }

                                while (textEnd < tagLength && (bytes[textEnd] == 0x00 && bytes[textEnd + 1] == 0x00) == false)
                                {
                                    textEnd += 2;
                                }

                                textLength = textEnd - textStart;
                                break;
                            default:
                                throw new ArgumentException("frame does not contain valid text data!", "frame");
                        }

                        if (textLength > 0)
                        {
                            Text = this.Encoding.GetString(bytes, textStart, textLength);
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

        public override string ToString()
        {
            return Text;
        }
    }
}
