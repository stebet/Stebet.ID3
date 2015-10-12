using System;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace StebetTagger.Core.Id3
{
    /// <summary>
    /// Represent an abstract class for a ID3 frame
    /// </summary>
    public abstract class Frame
    {
        protected Frame() { }

        public async Task WriteAsync(Stream stream, TagVersion version)
        {
            byte[] contentBytes = GetContentBytes(version);
            
            var tagId = Encoding.Default.GetBytes(GetTagId(version)); 
            await stream.WriteAsync(tagId, 0, tagId.Length).ConfigureAwait(false);
            switch (version)
            {
                case TagVersion.V23:
                    stream.WriteByte((byte)((contentBytes.Length / (256 * 256 * 256))));
                    stream.WriteByte((byte)((contentBytes.Length % (256 * 256 * 256)) / (256 * 256)));
                    stream.WriteByte((byte)((contentBytes.Length % (256 * 256)) / 256));
                    stream.WriteByte((byte)((contentBytes.Length % 256)));
                    break;
                case TagVersion.V24:
                    stream.WriteByte((byte)((contentBytes.Length / (128 * 128 * 128))));
                    stream.WriteByte((byte)((contentBytes.Length % (128 * 128 * 128)) / (128 * 128)));
                    stream.WriteByte((byte)((contentBytes.Length % (128 * 128)) / 128));
                    stream.WriteByte((byte)((contentBytes.Length % 128)));
                    break;
                default:
                    throw new ArgumentException("Unable to write frame with version = " + version.ToString(), nameof(version));
            }
            stream.WriteByte(0x00);
            stream.WriteByte(0x00);
            await stream.WriteAsync(contentBytes, 0, contentBytes.Length).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns the frame's contents as a byte array.
        /// </summary>
        /// <returns>The frame content as an array of bytes</returns>
        internal abstract byte[] GetContentBytes(TagVersion version);
        
        /// <summary>
        /// Returns the frames string ID depending on the tag version (if applicable)
        /// </summary>
        /// <param name="version">The TagVersion requested</param>
        /// <returns>A string corresponding to the Frame ID</returns>
        internal abstract string GetTagId(TagVersion version);

        public abstract Task FromStreamAsync(Stream stream, int tagLength, TagVersion version);
    }
}
