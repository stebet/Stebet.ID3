using System.Buffers;
using System.IO;

namespace StebetTagger.Core.Id3
{
    /// <summary>
    /// Header that describes an ID3 Version 2.3 Frame
    /// </summary>
    public class Frame23Header
    {
        public uint Id { get; private set; }
        public uint Size { get; private set; }
        public bool TagAlterPreservation { get; private set; }
        public bool FileAlterPreservation { get; private set; }
        public bool ReadOnly { get; private set; }
        public bool Compression { get; private set; }
        public bool Encryption { get; private set; }
        public bool GroupingIdentity { get; private set; }

        private Frame23Header()
        {
        }

        public static Frame23Header FromStream(Stream stream)
        {
            var frameHeader = ArrayPool<byte>.Shared.Rent(10);
            stream.Read(frameHeader, 0, 10);
            var header = new Frame23Header();

            header.Id = frameHeader.ReadUInt32(0);
            header.Size = frameHeader.ReadUInt32(4);
            header.TagAlterPreservation = ((frameHeader[8] & 0x80) == 0x80);
            header.FileAlterPreservation = ((frameHeader[8] & 0x40) == 0x40);
            header.ReadOnly = ((frameHeader[8] & 0x10) == 0x10);
            header.Compression = ((frameHeader[9] & 0x80) == 0x80);
            header.Encryption = ((frameHeader[9] & 0x40) == 0x40);
            header.GroupingIdentity = ((frameHeader[9] & 0x20) == 0x20);

            ArrayPool<byte>.Shared.Return(frameHeader);

            return header;
        }
    }

    public enum KnownIds
    {
        TALB = 0x54414C42
    }
}
