using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace StebetTagger.Core.Id3.Tags
{
    [Export(typeof(Frame))]
    public class UserDefinedText : Frame
    {
        public string Description { get; set; }
        public string Value { get; set; }

        internal override byte[] GetContentBytes(TagVersion version)
        {
            switch (version)
            {
                case TagVersion.V23:
                    List<byte> bytes = new List<byte>();
                    bytes.Add(0x00);
                    if (!string.IsNullOrEmpty(Description))
                    {
                        bytes.AddRange(Helpers.GetBytesFromString(Description));
                    }
                    bytes.Add(0x00);
                    if (!string.IsNullOrEmpty(Value))
                    {
                        bytes.AddRange(Helpers.GetBytesFromString(Value));
                    }
                    bytes.Add(0x00);
                    return bytes.ToArray();
                default:
                    throw new NotImplementedException("Writing " + version.ToString() + " has not been implemented!");
            }
        }

        internal override string GetTagId(TagVersion version)
        {
            return "TXXX";
        }

        public override void ReadBytes(byte[] frame, TagVersion version)
        {
            switch (version)
            {
                case TagVersion.V23:
                    if (frame.Length > 1)
                    {
                        int textStart = 1;
                        int textEnd = textStart;
                        if (frame[0] == 0x00)
                        {
                            while (textEnd < frame.Length && frame[textEnd] != 0x00)
                            {
                                textEnd++;
                            }
                            if (textEnd - textStart > 0)
                            {
                                Description = Helpers.GetStringFromBytes(frame, textStart, textEnd - textStart);
                                Value = Helpers.GetStringFromBytes(frame, textEnd + 1, frame.Length - (textEnd + 1));
                            }
                        }
                        else if (frame[0] == 0x01)
                        {
                            while (textEnd < frame.Length && (frame[textEnd] == 0x00 && frame[textEnd + 1] == 0x00) == false)
                            {
                                textEnd += 2;
                            }
                            if (textEnd - textStart > 0)
                            {
                                Description = Helpers.GetStringFromUnicodeBytes(frame, textStart, textEnd - textStart);
                                Value = Helpers.GetStringFromUnicodeBytes(frame, textEnd + 2, frame.Length - (textEnd + 2));
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

        public UserDefinedText(byte[] content, TagVersion version)
        {
            ReadBytes(content, version);
        }
        
        public override string ToString()
        {
            return Description + ":" + Value;
        }
    }
}
