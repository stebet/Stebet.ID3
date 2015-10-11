using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StebetTagger.Core.Id3
{
    internal static class Constants
    {
        internal static readonly string ID3Header = "ID3";
        internal static readonly byte[] ID3HeaderBytes = Encoding.Default.GetBytes("ID3");
    }
}
