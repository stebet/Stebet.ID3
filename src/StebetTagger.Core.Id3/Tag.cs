using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace StebetTagger.Core.Id3
{
    /// <summary>
    /// Represents a collection of ID3v2.4 frames
    /// </summary>
    public class Tag
    {
        public IList<Frame> Frames { get; }

        public Tag()
        {
            Frames = new List<Frame>();
        }

        public async Task Write(Stream stream, TagVersion version)
        {
            using (var memoryStream = new MemoryStream())
            {
                foreach (Frame id3Frame in Frames)
                {
                    await id3Frame.WriteAsync(memoryStream, version);
                }

                await stream.WriteAsync(Constants.ID3Header, 0, Constants.ID3Header.Length);
                switch (version)
                {
                    case TagVersion.V22:
                        stream.WriteByte(0x02);
                        break;
                    case TagVersion.V23:
                        stream.WriteByte(0x03);
                        break;
                    case TagVersion.V24:
                        stream.WriteByte(0x04);
                        break;
                    default:
                        throw new ArgumentException("Unable to write ID3 Tag with version = " + version.ToString(), nameof(version));
                }
                stream.WriteByte(0x00);
                stream.WriteByte(0x00);

                long size = memoryStream.Length;
                stream.WriteByte((byte)(size / (128 * 128 * 128)));
                stream.WriteByte((byte)((size % (128 * 128 * 128)) / (128 * 128)));
                stream.WriteByte((byte)((size % (128 * 128)) / 128));
                stream.WriteByte((byte)(size % 128));

                await memoryStream.CopyToAsync(stream);
            }
        }
    }
}
