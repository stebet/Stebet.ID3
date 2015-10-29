using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StebetTagger.Core.Id3.Tags
{
    [DebuggerDisplay("{OwnerIdentifier} : byte[{Identifier.Length}]")]
    public class UniqueFileIdentifier : Frame
    {
        public string OwnerIdentifier { get; set; }
        public byte[] Identifier { get; set; }

        internal override byte[] GetContentBytes(TagVersion version)
        {
            List<byte> bytes = new List<byte>();
            switch (version)
            {
                case TagVersion.V23:
                    bytes.AddRange(Encoding.Default.GetBytes(OwnerIdentifier));
                    bytes.Add(0x00);
                    bytes.AddRange(Identifier);
                    break;
                case TagVersion.V24:
                default:
                    throw new ArgumentException("Version " + version.ToString() + " is not supported for this frame!", nameof(version));
            }
            return bytes.ToArray();
        }

        internal override string GetTagId(TagVersion version) => "UFID";

        public override async Task FromStreamAsync(Stream stream, int tagLength, TagVersion version)
        {
            if (tagLength > 3)
            {
                long streamStart = stream.Position;
                OwnerIdentifier = stream.ReadAnsiString(streamStart + tagLength);
                Identifier = new byte[tagLength - (stream.Position - streamStart)];
                await stream.ReadAsync(Identifier, 0, Identifier.Length).ConfigureAwait(false);
            }
            else
            {
                throw new ArgumentException("Frame has an invalid length", "frame");
            }
        }
    }
}
