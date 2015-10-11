using System;

namespace StebetTagger.Core.Id3.Tags
{
    public class AlbumArtist : TextFrame
    {
        internal override string GetTagId(TagVersion version)
        {
            switch (version)
            {
                case TagVersion.V23:
                    return "TPE2";
                default:
                    throw new NotImplementedException("Frame{" + ToString() + "} has not been implemented for version " + version.ToString());
            }
        }

        public AlbumArtist()
        {
        }
    }
}