using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace StebetTagger.Core.Id3.Tags
{
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
                    throw new ArgumentException("Version " + version.ToString() + " is not supported for this frame!", nameof(version));
            }
        }

        public override async Task FromStreamAsync(Stream stream, uint tagLength, TagVersion version)
        {
            long streamStart = stream.Position;
            int encoding = stream.ReadByte();
            switch (version)
            {
                case TagVersion.V24:
                case TagVersion.V23:
                    if (encoding == 0x00)
                    {
                        MimeType = await stream.ReadAnsiString(streamStart + tagLength).ConfigureAwait(false);
                        PictureType = (AttachedPictureType)stream.ReadByte();

                        Description = await stream.ReadAnsiString(streamStart + tagLength).ConfigureAwait(false);
                        Data = new byte[tagLength - (stream.Position - streamStart)];
                        await stream.ReadAsync(Data, 0, Data.Length).ConfigureAwait(false);
                    }
                    else if (encoding == 0x01)
                    {
                        MimeType = await stream.ReadAnsiString(streamStart + tagLength).ConfigureAwait(false);
                        PictureType = (AttachedPictureType)stream.ReadByte();
                        Description = await stream.ReadUnicodeStringAsync(streamStart + tagLength).ConfigureAwait(false);
                        Data = new byte[tagLength - (stream.Position - streamStart)];
                        await stream.ReadAsync(Data, 0, Data.Length).ConfigureAwait(false);
                    }
                    break;
                default:
                    Debug.WriteLine("APIC: Version not implemented - " + version.ToString());
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
