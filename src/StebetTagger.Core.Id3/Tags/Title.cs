using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace StebetTagger.Core.Id3.Tags
{
    [Export("TIT2", typeof(Frame))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class Title : TextFrame
    {
        internal override string GetTagId(TagVersion version)
        {
            switch (version)
            {
                case TagVersion.V23:
                    return "TIT2";
                default:
                    throw new NotImplementedException("Frame{" + this.ToString() + "} has not been implemented for version " + version.ToString());
            }
        }

        public Title()
        {
        }
    }
}
