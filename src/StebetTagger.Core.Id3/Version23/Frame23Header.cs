using System.IO;
using System.Threading.Tasks;

namespace StebetTagger.Core.Id3
{
    /// <summary>
    /// Header that describes an ID3 Version 2.3 Frame
    /// </summary>
    public class Frame23Header
	{
        public string Id { get; private set; }
        public int Size { get; private set; }
        public bool TagAlterPreservation { get; private set; }
        public bool FileAlterPreservation { get; private set; }
        public bool ReadOnly { get; private set; }
        public bool Compression { get; private set; }
        public bool Encryption { get; private set; }
        public bool GroupingIdentity { get; private set; }
		
		private Frame23Header()
		{
		}

        public static async Task<Frame23Header> FromStream(Stream stream)
        {
            var frameHeader = new byte[10];
            await stream.ReadAsync(frameHeader, 0, frameHeader.Length);
            var header = new Frame23Header();

            header.Id = ((char)frameHeader[0]).ToString() + ((char)frameHeader[1]).ToString() + ((char)frameHeader[2]).ToString() + ((char)frameHeader[3]).ToString();
            header.Size = (int)frameHeader[4] * 256 * 256 * 256 + (int)frameHeader[5] * 256 * 256 + (int)frameHeader[6] * 256 + (int)frameHeader[7];
            header.TagAlterPreservation = ((frameHeader[8] & 0x80) == 0x80);
            header.FileAlterPreservation = ((frameHeader[8] & 0x40) == 0x40);
            header.ReadOnly = ((frameHeader[8] & 0x10) == 0x20);
            header.Compression = ((frameHeader[9] & 0x80) == 0x80);
            header.Encryption = ((frameHeader[9] & 0x40) == 0x40);
            header.GroupingIdentity = ((frameHeader[9] & 0x20) == 0x20);

            return header;
        }
	}
}
