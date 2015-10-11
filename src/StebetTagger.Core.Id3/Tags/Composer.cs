using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace StebetTagger.Core.Id3.Tags
{
    [Export("TCOM", typeof(Frame))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class Composer : TextFrame
    {
        internal override string GetTagId(TagVersion version)
        {
            switch (version)
            {
                case TagVersion.V23:
                    return "TCOM";
                default:
                    throw new NotImplementedException("Frame{" + this.ToString() + "} has not been implemented for version " + version.ToString());
            }
        }

        public Composer()
        {
        }
    }
}