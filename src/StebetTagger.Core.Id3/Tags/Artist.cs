using System;

namespace StebetTagger.Core.Id3.Tags
{
    public class Artist : TextFrame
    {
        internal override string GetTagId(TagVersion version)
        {
            switch (version)
            {
                case TagVersion.V23:
                    return "TPE1";
                default:
                    throw new NotImplementedException("Frame{" + ToString() + "} has not been implemented for version " + version.ToString());
            }
        }

        public Artist()
        {
        }
    }
}