using System;
using System.Buffers;
using System.IO;
using System.Threading.Tasks;

namespace StebetTagger.Core.Id3
{
    public class TagHeader
    {
        public TagVersion Version { get; private set; }
        public bool Unsynchronization { get; private set; }
        public bool ExtendedHeader { get; private set; }
        public bool Experimental { get; private set; }
        public bool HasFooter { get; private set; }
        public int TagLength { get; private set; }

        private TagHeader()
        {
        }

        internal static async Task<TagHeader> ReadTagHeader(Stream stream)
        {
            var header = new TagHeader();
            byte[] headerBytes = ArrayPool<byte>.Shared.Rent(7);
            await stream.ReadAsync(headerBytes, 0, 7).ConfigureAwait(false);

            switch (Convert.ToInt32(headerBytes[0]))
            {
                case 2:
                    header.Version = TagVersion.V22;
                    break;
                case 3:
                    header.Version = TagVersion.V23;
                    break;
                case 4:
                    header.Version = TagVersion.V24;
                    break;
                default:
                    header.Version = TagVersion.Unknown;
                    break;
            }

            header.Unsynchronization = ((headerBytes[2] & 0x80) == 0x80);
            header.ExtendedHeader = ((headerBytes[2] & 0x40) == 0x40);
            header.Experimental = ((headerBytes[2] & 0x20) == 0x20);
            header.HasFooter = ((headerBytes[2] & 0x10) == 0x10);
            header.TagLength = headerBytes[3] * 128 * 128 * 128 + headerBytes[4] * 128 * 128 + headerBytes[5] * 128 + headerBytes[6];
            ArrayPool<byte>.Shared.Return(headerBytes);
            return header;
        }
    }
}