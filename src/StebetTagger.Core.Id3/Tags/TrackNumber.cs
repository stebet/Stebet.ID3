using System;

namespace StebetTagger.Core.Id3.Tags
{
    public class TrackNumber : TextFrame
    {
        internal override string GetTagId(TagVersion version)
        {
            switch (version)
            {
                case TagVersion.V23:
                    return "TRCK";
                default:
                    throw new NotImplementedException("Frame{" + ToString() + "} has not been implemented for version " + version.ToString());
            }
        }

        public TrackNumber()
        {
        }
    }
}