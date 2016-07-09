using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace StebetTagger.Core.Id3.Tests
{
    [TestClass]
    public class StreamExtensionTests
    {
        [TestMethod]
        public async Task ReadAnsiStringAndReachEndOfStream()
        {
            byte[] ansiBytes = Encoding.ASCII.GetBytes("This Is An Ansi String");
            var memoryStream = new MemoryStream(ansiBytes);
            Assert.AreEqual("This Is An Ansi String", await memoryStream.ReadAnsiString(ansiBytes.LongLength - 1));
        }

        [TestMethod]
        public async Task ReadAnsiStringAndReachNullTermination()
        {
            byte[] ansiBytes = Encoding.ASCII.GetBytes("This Is An Ansi String\0\0\0");
            var memoryStream = new MemoryStream(ansiBytes);
            Assert.AreEqual("This Is An Ansi String", await memoryStream.ReadAnsiString(ansiBytes.LongLength - 1));
        }

        [TestMethod]
        public void ReadUnicodeString()
        {
        }
    }
}
