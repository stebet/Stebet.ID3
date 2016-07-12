using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StebetTagger.Core.Id3.Tests
{
    [TestClass]
    public class StreamExtensionTests
    {
        private readonly string Test = "Test";

        [TestMethod]
        public async Task ReadAnsiStringAndReachEndOfStream()
        {
            byte[] ansiBytes = Encoding.ASCII.GetBytes(Test);
            var memoryStream = new MemoryStream(ansiBytes);
            Assert.AreEqual(Test, await memoryStream.ReadAnsiString(ansiBytes.LongLength));
        }

        [TestMethod]
        public async Task ReadAnsiStringAndReachNullTermination()
        {
            byte[] ansiBytes = Encoding.ASCII.GetBytes(Test + "\0\0\0");
            var memoryStream = new MemoryStream(ansiBytes);
            Assert.AreEqual(Test, await memoryStream.ReadAnsiString(ansiBytes.LongLength));
        }

        [TestMethod]
        public async Task ReadTwoAnsiStringsSeparatedByNullTermination()
        {
            byte[] ansiBytes = Encoding.ASCII.GetBytes(Test + "\0" + Test);
            var memoryStream = new MemoryStream(ansiBytes);
            Assert.AreEqual(Test, await memoryStream.ReadAnsiString(ansiBytes.LongLength));
            Assert.AreEqual(Test, await memoryStream.ReadAnsiString(ansiBytes.LongLength));
        }

        [TestMethod]
        public async Task ReadUnicodeStringAndReachEndOfStream()
        {
            byte[] ansiBytes = Encoding.Unicode.GetPreamble().Concat(Encoding.Unicode.GetBytes(Test)).ToArray();
            var memoryStream = new MemoryStream(ansiBytes);
            Assert.AreEqual(Test, await memoryStream.ReadUnicodeStringAsync(ansiBytes.LongLength));
        }

        [TestMethod]
        public async Task ReadUnicodeStringAndReachNullTermination()
        {
            byte[] ansiBytes = Encoding.Unicode.GetPreamble().Concat(Encoding.Unicode.GetBytes(Test + "\0Garbage")).ToArray();
            var memoryStream = new MemoryStream(ansiBytes);
            Assert.AreEqual(Test, await memoryStream.ReadUnicodeStringAsync(ansiBytes.LongLength));
        }

        [TestMethod]
        public async Task ReadTwoUnicodeStringsSeparatedByNullTermination()
        {
            byte[] ansiBytes = Encoding.Unicode.GetPreamble().Concat(Encoding.Unicode.GetBytes(Test + "\0")).Concat(Encoding.Unicode.GetPreamble()).Concat(Encoding.Unicode.GetBytes(Test)).ToArray();
            var memoryStream = new MemoryStream(ansiBytes);
            Assert.AreEqual(Test, await memoryStream.ReadUnicodeStringAsync(ansiBytes.LongLength));
            Assert.AreEqual(Test, await memoryStream.ReadUnicodeStringAsync(ansiBytes.LongLength));
        }

        [TestMethod]
        public async Task ReadBigEndianUnicodeStringAndReachEndOfStream()
        {
            byte[] ansiBytes = Encoding.BigEndianUnicode.GetPreamble().Concat(Encoding.BigEndianUnicode.GetBytes(Test)).ToArray();
            var memoryStream = new MemoryStream(ansiBytes);
            Assert.AreEqual(Test, await memoryStream.ReadUnicodeStringAsync(ansiBytes.LongLength));
        }

        [TestMethod]
        public async Task ReadBigEndianUnicodeStringAndReachNullTermination()
        {
            byte[] ansiBytes = Encoding.BigEndianUnicode.GetPreamble().Concat(Encoding.BigEndianUnicode.GetBytes(Test + "\0Garbage")).ToArray();
            var memoryStream = new MemoryStream(ansiBytes);
            Assert.AreEqual(Test, await memoryStream.ReadUnicodeStringAsync(ansiBytes.LongLength));
        }

        [TestMethod]
        public async Task ReadTwoBigEndianUnicodeStringsSeparatedByNullTermination()
        {
            byte[] ansiBytes = Encoding.BigEndianUnicode.GetPreamble().Concat(Encoding.BigEndianUnicode.GetBytes(Test + "\0")).Concat(Encoding.BigEndianUnicode.GetPreamble()).Concat(Encoding.BigEndianUnicode.GetBytes(Test)).ToArray();
            var memoryStream = new MemoryStream(ansiBytes);
            Assert.AreEqual(Test, await memoryStream.ReadUnicodeStringAsync(ansiBytes.LongLength));
            Assert.AreEqual(Test, await memoryStream.ReadUnicodeStringAsync(ansiBytes.LongLength));
        }
    }
}
