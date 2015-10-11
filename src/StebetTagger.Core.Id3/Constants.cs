using System.Text;

namespace StebetTagger.Core.Id3
{
    internal static class Constants
    {
        internal static readonly string ID3Header = "ID3";
        internal static readonly byte[] ID3HeaderBytes = Encoding.Default.GetBytes("ID3");
    }
}
