using StebetTagger.Core.Id3.Tags;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace StebetTagger.Core.Id3
{
    /// <summary>
    /// This class creates instances of ID3 frames based on the V2.4 specification from a byte array.
    /// </summary>
    public static class Tag23Factory
    {
        private static readonly uint TALB = Encoding.ASCII.GetBytes("TALB").ReadUInt32(0);
        private static readonly uint TPE1 = Encoding.ASCII.GetBytes("TPE1").ReadUInt32(0);
        private static readonly uint TPE2 = Encoding.ASCII.GetBytes("TPE2").ReadUInt32(0);
        private static readonly uint TPE3 = Encoding.ASCII.GetBytes("TPE3").ReadUInt32(0);
        private static readonly uint TCOM = Encoding.ASCII.GetBytes("TCOM").ReadUInt32(0);
        private static readonly uint TCON = Encoding.ASCII.GetBytes("TCON").ReadUInt32(0);
        private static readonly uint TIT2 = Encoding.ASCII.GetBytes("TIT2").ReadUInt32(0);
        private static readonly uint TRCK = Encoding.ASCII.GetBytes("TRCK").ReadUInt32(0);
        private static readonly uint TYER = Encoding.ASCII.GetBytes("TYER").ReadUInt32(0);
        private static readonly uint APIC = Encoding.ASCII.GetBytes("APIC").ReadUInt32(0);
        private static readonly uint USLT = Encoding.ASCII.GetBytes("USLT").ReadUInt32(0);
        private static readonly uint TXXX = Encoding.ASCII.GetBytes("TXXX").ReadUInt32(0);
        private static readonly uint TIT3 = Encoding.ASCII.GetBytes("TIT3").ReadUInt32(0);
        private static readonly uint TENC = Encoding.ASCII.GetBytes("TENC").ReadUInt32(0);
        private static readonly uint TORY = Encoding.ASCII.GetBytes("TORY").ReadUInt32(0);
        private static readonly uint TPUB = Encoding.ASCII.GetBytes("TPUB").ReadUInt32(0);
        private static readonly uint TPOS = Encoding.ASCII.GetBytes("TPOS").ReadUInt32(0);
        private static readonly uint TOWN = Encoding.ASCII.GetBytes("TOWN").ReadUInt32(0);
        private static readonly uint PRIV = Encoding.ASCII.GetBytes("PRIV").ReadUInt32(0);
        private static readonly uint TOFN = Encoding.ASCII.GetBytes("TOFN").ReadUInt32(0);
        private static readonly uint TSRC = Encoding.ASCII.GetBytes("TSRC").ReadUInt32(0);
        private static readonly uint TCOP = Encoding.ASCII.GetBytes("TCOP").ReadUInt32(0);
        private static readonly uint TDAT = Encoding.ASCII.GetBytes("TDAT").ReadUInt32(0);
        private static readonly uint TMED = Encoding.ASCII.GetBytes("TMED").ReadUInt32(0);
        private static readonly uint UFID = Encoding.ASCII.GetBytes("UFID").ReadUInt32(0);

        public static async Task<Tag> FromStream(Stream stream, int tagLength)
        {
            var newTag = new Tag();
            while (stream.Position < tagLength && stream.ReadByte() != 0x00)
            {
                stream.Seek(-1, SeekOrigin.Current);
                Frame23Header frameHeader = Frame23Header.FromStream(stream);

                Frame tag = GetFrame(frameHeader.Id);
                if (tag != null)
                {
                    await tag.FromStreamAsync(stream, frameHeader.Size, TagVersion.V23).ConfigureAwait(false);
                    newTag.Frames.Add(tag);
                }
                else
                {
                    Debug.WriteLine($"Unable to read {frameHeader.Id} tag. Skipping it.");
                    stream.Seek(frameHeader.Size, SeekOrigin.Current);
                }
            }

            return newTag;
        }

        public static Frame GetFrame(uint frameId)
        {
            if (frameId == TALB)
                return new Album();
            if (frameId == TPE1)
                return new Artist();
            if (frameId == TPE2 || frameId == TPE3)
                return new AlbumArtist();
            if (frameId == TCOM)
                return new Composer();
            if (frameId == TCON)
                return new ContentType();
            if (frameId == TIT2)
                return new Title();
            if (frameId == TRCK)
                return new TrackNumber();
            if (frameId == TYER)
                return new Year();
            if (frameId == APIC)
                return new AttachedPicture();
            if (frameId == USLT)
                return new UnsynchronizedLyrics();
            if (frameId == TXXX)
                return new UserDefinedText();
            if (frameId == TIT3)
                return new Subtitle();
            if (frameId == TENC)
                return new EncodedBy();
            if (frameId == TORY)
                return new OriginalYear();
            if (frameId == TPUB)
                return new Publisher();
            if (frameId == TPOS)
                return new PartOfSet();
            if (frameId == TOWN)
                return new Owner();
            if (frameId == PRIV)
                return new Private();
            if (frameId == TOFN)
                return new OriginalFilename();
            if (frameId == TSRC)
                return new InternationalStandardRecordingCode();
            if (frameId == TCOP)
                return new Copyright();
            if (frameId == TDAT)
                return new Date();
            if (frameId == TMED)
                return new MediaType();
            if (frameId == UFID)
                return new UniqueFileIdentifier();

            return null;
        }
    }
}