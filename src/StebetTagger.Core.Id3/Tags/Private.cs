using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StebetTagger.Core.Id3.Tags
{
    [DebuggerDisplay("{Description} : byte[{Value.Length}]")]
    public class Private : Frame
    {
        public string Description { get; set; }
        public byte[] Value { get; set; }

        public override async Task FromStreamAsync(Stream stream, uint tagLength, TagVersion version)
        {
            switch (version)
            {
                case TagVersion.V23:
                    if (tagLength > 1)
                    {
                        long streamStart = stream.Position;
                        Description = await stream.ReadAnsiString(streamStart + tagLength).ConfigureAwait(false);
                        Value = new byte[tagLength - (stream.Position - streamStart)];
                        await stream.ReadAsync(Value, 0, Value.Length).ConfigureAwait(false);
                    }
                    else
                    {
                        throw new ArgumentException("frame does not contain valid data!", "frame");
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
                    bytes.AddRange(Encoding.Default.GetBytes(Description));
                    bytes.Add(0x00);
                    bytes.AddRange(Value);
                    return bytes.ToArray();
                default:
                    throw new NotImplementedException("Writing " + version.ToString() + " has not been implemented!");
            }
        }

        internal override string GetTagId(TagVersion version) => "PRIV";
    }
}
