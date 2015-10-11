﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace StebetTagger.Core.Id3.Tags
{
    [Export("TPE2", typeof(Frame))]
    [Export("TPE3", typeof(Frame))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class AlbumArtist : TextFrame
    {
        internal override string GetTagId(TagVersion version)
        {
            switch (version)
            {
                case TagVersion.V23:
                    return "TPE2";
                default:
                    throw new NotImplementedException("Frame{" + this.ToString() + "} has not been implemented for version " + version.ToString());
            }
        }

        public AlbumArtist()
        {
        }
    }
}