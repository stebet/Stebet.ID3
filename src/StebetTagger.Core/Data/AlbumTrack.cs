using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StebetTagger.Core.Data
{
    public class AlbumTrack
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public string Artist { get; set; }
        public string Lyrics { get; set; }
        public AlbumDisc Disc { get; set; }
    }
}
