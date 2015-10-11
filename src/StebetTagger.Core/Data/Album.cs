using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace StebetTagger.Core.Data
{
    public class Album
    {
        public Collection<AlbumDisc> Discs { get; private set; }
        public string Artist { get; set; }
        public string Title { get; set; }
        private byte[] image;
        public string ImageType { get; set; }
        public DateTime OriginalReleaseDate { get; set; }

        public Album()
        {
            Discs = new Collection<AlbumDisc>();
        }

        public byte[] GetImage()
        {
            return image;
        }

        public void SetImage(byte[] imageData)
        {
            image = imageData;
        }
    }
}
