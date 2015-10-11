using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.ComponentModel.Composition;

namespace StebetTagger.Core.Id3.Tags
{
    [Export(typeof(Frame))]
    public class AttachedPicture : Frame
    {
        public AttachedPictureType PictureType { get; set; }
        public string MimeType { get; set; }
        public string Description { get; set; }
        private byte[] _data;

        public byte[] Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public AttachedPicture(byte[] frame, TagVersion version)
        {
            this.ReadBytes(frame, version);
        }

        public AttachedPicture()
        {
        }

        internal override byte[] GetContentBytes(TagVersion version)
        {
            List<byte> bytes = new List<byte>();

            bytes.Add(0x00);
            bytes.AddRange(System.Text.Encoding.Default.GetBytes(this.MimeType));
            bytes.Add(0x00);
            bytes.Add((byte)this.PictureType);
            if (!String.IsNullOrEmpty(this.Description))
            {
                bytes.AddRange(System.Text.Encoding.Default.GetBytes(this.Description));
            }
            bytes.Add(0x00);
            bytes.AddRange(this.Data);

            return bytes.ToArray();
        }

        internal override string GetTagId(TagVersion version)
        {
            switch (version)
            {
                case TagVersion.V23:
                case TagVersion.V24:
                    return "APIC";
                default:
                    throw new ArgumentException("Version " + version.ToString() + " is not supported for this frame!", "version");
            }
        }

        public override void ReadBytes(byte[] frame, TagVersion version)
        {
            switch (version)
            {
                case TagVersion.V24:
                case TagVersion.V23:
                    if (frame[0] == 0x00)
                    {
                        int endMimeType = 1;
                        while (endMimeType < frame.Length && frame[endMimeType] != 0x00)
                        {
                            endMimeType++;
                        }
                        PictureType = (AttachedPictureType)frame[endMimeType + 1];

                        int endDescription = endMimeType + 2;
                        while (endDescription < frame.Length && frame[endDescription] != 0x00)
                        {
                            endDescription++;
                        }

                        MimeType = System.Text.Encoding.Default.GetString(frame, 1, endMimeType - 1);
                        if (endDescription > (endMimeType + 2))
                        {
                            Description = System.Text.Encoding.Default.GetString(frame, endMimeType + 2, endDescription - 1);
                        }
                        Data = new byte[frame.Length - (endDescription + 1)];
                        Array.Copy(frame, endDescription + 1, Data, 0, Data.Length);
                    }
                    else if (frame[0] == 0x01)
                    {
                        int endMimeType = 1;
                        while (endMimeType < frame.Length && frame[endMimeType] != 0x00)
                        {
                            endMimeType++;
                        }
                        PictureType = (AttachedPictureType)frame[endMimeType + 1];

                        int endDescription = endMimeType + 2;
                        while (endDescription < frame.Length && !(frame[endDescription] == 0x00 && frame[endDescription + 1] == 0x00))
                        {
                            endDescription+=2;
                        }

                        MimeType = System.Text.Encoding.Default.GetString(frame, 1, endMimeType - 1);

                        if (endDescription > (endMimeType + 2))
                        {
                            if (frame[endMimeType + 2] == 0xFF && frame[endMimeType + 3] == 0xFE)
                            {
                                Description = System.Text.Encoding.Unicode.GetString(frame, endMimeType + 4, endDescription - 2);
                            }
                            else
                            {
                                Description = System.Text.Encoding.BigEndianUnicode.GetString(frame, endMimeType + 4, endDescription - 2);
                            }
                        }
                        Data = new byte[frame.Length - (endDescription + 2)];
                        Array.Copy(frame, endDescription + 2, Data, 0, Data.Length);
                    }
                    break;
                default:
                    Debug.WriteLine("TXXX: Version not implemented - " + version.ToString());
                    break;
            }
        }
    }

    public enum AttachedPictureType
    {
        Other = 0x00,
        FileIcon = 0x01,
        OtherFileIcon = 0x02,
        CoverFront = 0x03,
        CoverBack = 0x04,
        LeafletPage = 0x05,
        Media = 0x06,
        LeadArtist = 0x07,
        Artist = 0x08,
        Conductor = 0x09,
        Band = 0x0A,
        Composer = 0x0B,
        Lyricist = 0x0C,
        RecordingLocation = 0x0D,
        DuringRecording = 0x0E,
        DuringPerformance = 0x0F,
        VideoScreenCapture = 0x10,
        ABrightColouredFish = 0x11,
        Illustration = 0x12,
        ArtistLogotype = 0x13,
        StudioLogotype = 0x14
    }
}
