using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using StebetTagger.Core.Data;

namespace StebetTagger.Core
{
    public class RatedTrack
    {
        public AlbumTrack Track { get; set; }
        public double Rating { get; set; }
    }
}
