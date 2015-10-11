using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace StebetTagger.Core.Id3.Tags
{
    [Export(typeof(Frame))]
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
                    bytes.AddRange(Helpers.GetBytesFromString("eng"));
                    if (!string.IsNullOrEmpty(Description))
                    {
                        bytes.AddRange(Helpers.GetBytesFromString(Description));
                    }
                    bytes.Add(0x00);
                    if (!string.IsNullOrEmpty(Text))
                    {
                        bytes.AddRange(Helpers.GetBytesFromString(Text));
                    }
                    bytes.Add(0x00);
                    break;
                case TagVersion.V24:
                default:
                    throw new ArgumentException("Version " + version.ToString() + " is not supported for this frame!", "version");
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
                    throw new ArgumentException("Version " + version.ToString() + " is not supported for this frame!", "version");
            }
        }

        public override void ReadBytes(byte[] frame, TagVersion version)
        {
            if (frame.Length > 6)
            {
                Language = Convert.ToChar(frame[1]).ToString() + Convert.ToChar(frame[2]).ToString() + Convert.ToChar(frame[3]).ToString();
                int descriptionStart = 4;
                int descriptionEnd = descriptionStart;
                int textStart = 0;
                int textEnd = 0;
                switch (frame[0])
                {
                    case 0x00:
                        while(frame[descriptionEnd] != 0x00)
                        {
                            descriptionEnd++;
                        }
                        textStart = descriptionEnd + 1;
                        textEnd = textStart;
                        while (frame[textEnd] != 0x00 && textEnd < (frame.Length - 1))
                        {
                            textEnd++;
                        }
                        if (descriptionEnd - descriptionStart > 0)
                        {
                            Description = Helpers.GetStringFromBytes(frame, descriptionStart, descriptionEnd - descriptionStart);
                        }
                        if (textEnd - textStart > 0)
                        {
                            Text = Helpers.GetStringFromBytes(frame, textStart, textEnd - textStart);
                        }
                        break;
                    case 0x01:
                        break;
                    default:
                        throw new ArgumentException("Frame has an invalid encoding marker", "frame");
                }
            }
        }

        public UnsynchronizedLyrics(byte[] content, TagVersion version)
        {
            ReadBytes(content, version);
        }

        public UnsynchronizedLyrics()
        {
        }
    }
}
