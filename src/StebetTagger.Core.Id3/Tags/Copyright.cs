using System;

namespace StebetTagger.Core.Id3.Tags
{
    public class Copyright : TextFrame
    {
        internal override string GetTagId(TagVersion version)
        {
            switch (version)
            {
                case TagVersion.V23:
                    return "TCOP";
                default:
                    throw new NotImplementedException("Frame{" + ToString() + "} has not been implemented for version " + version.ToString());
            }
        }

        public Copyright()
        {
        }
    }
}
