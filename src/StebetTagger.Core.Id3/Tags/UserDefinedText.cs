using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StebetTagger.Core.Id3.Tags
{
    [DebuggerDisplay("{Description}: {Value}")]
    public class UserDefinedText : Frame
    {
        public string Description { get; set; }
        public string Value { get; set; }

        public override async Task FromStreamAsync(Stream stream, uint tagLength, TagVersion version)
        {
            switch (version)
            {
                case TagVersion.V23:
                    if (tagLength > 1)
                    {
                        long streamStart = stream.Position;
                        int encoding = stream.ReadByte();
                        if (encoding == 0x00)
                        {
                            Description = await stream.ReadAnsiString(streamStart + tagLength).ConfigureAwait(false);
                            Value = await stream.ReadAnsiString(streamStart + tagLength).ConfigureAwait(false);
                        }
                        else if (encoding == 0x01)
                        {
                            Description = await stream.ReadUnicodeStringAsync(streamStart + tagLength).ConfigureAwait(false);
                            Value = await stream.ReadUnicodeStringAsync(streamStart + tagLength).ConfigureAwait(false);
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

        internal override byte[] GetContentBytes(TagVersion version)
        {
            switch (version)
            {
                case TagVersion.V23:
                    List<byte> bytes = new List<byte>();
                    bytes.Add(0x00);
                    if (!string.IsNullOrEmpty(Description))
                    {
                        bytes.AddRange(Encoding.Default.GetBytes(Description));
                    }
                    bytes.Add(0x00);
                    if (!string.IsNullOrEmpty(Value))
                    {
                        bytes.AddRange(Encoding.Default.GetBytes(Value));
                    }
                    bytes.Add(0x00);
                    return bytes.ToArray();
                default:
                    throw new NotImplementedException("Writing " + version.ToString() + " has not been implemented!");
            }
        }

        internal override string GetTagId(TagVersion version) => "TXXX";
    }
}
