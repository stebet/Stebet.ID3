using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StebetTagger.Core.Data
{
    public class SearchResult
    {
        public string AlbumArtist { get; set; }
        public string AlbumTitle { get; set; }
        public bool HasLyrics { get; set; }
        public bool HasAlbumArt { get; set; }
        public string Id { get; set; }

        public override string ToString()
        {
            return AlbumArtist + " - " + AlbumTitle;
        }
    }
}
