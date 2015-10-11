using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace StebetTagger.Core.Data
{
    public class AlbumDisc
    {
        public int Number { get; set; }
        public Collection<AlbumTrack> Tracks { get; private set; }
        public Album Album {get; set;}

        public AlbumDisc()
        {
            Tracks = new Collection<AlbumTrack>();
        }
    }
}
