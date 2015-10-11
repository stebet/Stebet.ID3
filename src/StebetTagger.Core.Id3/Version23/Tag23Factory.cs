using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using StebetTagger.Core.Id3.Tags;

namespace StebetTagger.Core.Id3
{
    /// <summary>
    /// This class creates instances of ID3 frames based on the V2.4 specification from a byte array.
    /// </summary>
    public static class Tag23Factory
    {
        public static async Task<Tag> FromStream(Stream stream, int tagLength)
        {
            Tag newTag = new Tag();
            int peek = stream.ReadByte();
            while (stream.Position < tagLength && peek != 0x00)
            {
                stream.Seek(-1, SeekOrigin.Current);
                Frame23Header frameHeader = await Frame23Header.FromStream(stream);

                Frame tag = GetFrame(frameHeader.Id);
                if (tag != null)
                {
                    var frameBytes = new byte[frameHeader.Size];
                    await stream.ReadAsync(frameBytes, 0, frameBytes.Length);
                    tag.FromBytes(frameBytes, frameHeader.Size, TagVersion.V23);
                    newTag.Frames.Add(tag);
                }
                else
                {
                    Debug.WriteLine("Unable to read {0} tag. Skipping it.", frameHeader.Id);
                    stream.Seek(frameHeader.Size, SeekOrigin.Current);
                }

                peek = stream.ReadByte();
            }

            return newTag;
        }

        public static Frame GetFrame(string frameId)
        {
            switch(frameId)
            {
                case "TALB":
                    return new Album();
                case "TPE1":
                    return new Artist();
                case "TPE2":
                case "TPE3":
                    return new AlbumArtist();
                case "TCOM":
                    return new Composer();
                case "TCON":
                    return new ContentType();
                case "TIT2":
                    return new Title();
                case "TRCK":
                    return new TrackNumber();
                case "TYER":
                    return new Year();
                default:
                    return null;
            }
        }
    }
}