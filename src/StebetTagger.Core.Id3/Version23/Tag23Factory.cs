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
            var newTag = new Tag();
            while (stream.Position < tagLength && stream.ReadByte() != 0x00)
            {
                stream.Seek(-1, SeekOrigin.Current);
                Frame23Header frameHeader = await Frame23Header.FromStream(stream).ConfigureAwait(false);

                Frame tag = GetFrame(frameHeader.Id);
                if (tag != null)
                {
                    await tag.FromStreamAsync(stream, frameHeader.Size, TagVersion.V23).ConfigureAwait(false);
                    newTag.Frames.Add(tag);
                }
                else
                {
                    Debug.WriteLine($"Unable to read {frameHeader.Id} tag. Skipping it.");
                    stream.Seek(frameHeader.Size, SeekOrigin.Current);
                }
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