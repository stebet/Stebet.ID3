using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace StebetTagger.Core.Id3.Tags
{
    [Export(typeof(Frame))]
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
                    bytes.AddRange(Helpers.GetBytesFromString(OwnerIdentifier));
                    bytes.Add(0x00);
                    bytes.AddRange(Identifier);
                    break;
                case TagVersion.V24:
                default:
                    throw new ArgumentException("Version " + version.ToString() + " is not supported for this frame!", "version");
            }
            return bytes.ToArray();
        }

        internal override string GetTagId(TagVersion version)
        {
            return "UFID";
        }

        public override void ReadBytes(byte[] frame, TagVersion version)
        {
            if (frame.Length > 3)
            {
                int terminator = 0;
                for (int i = 0; i < frame.Length; i++)
                {
                    if (frame[i] != 0x00)
                    {
                        terminator++;
                    }
                    else
                    {
                        break;
                    }
                }
                this.OwnerIdentifier = Helpers.GetStringFromBytes(frame, 0, terminator);
                this.Identifier = new byte[frame.Length - (terminator + 1)];
                Array.Copy(frame, terminator + 1, this.Identifier, 0, this.Identifier.Length);
            }
            else
            {
                throw new ArgumentException("Frame has an invalid length", "frame");
            }
        }

        public UniqueFileIdentifier(byte[] content, TagVersion version)
        {
            ReadBytes(content, version);
        }
    }
}
