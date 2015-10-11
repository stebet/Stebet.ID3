using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StebetTagger.Core.Id3
{
    public class MP3File
    {
        public static byte[] Id3V2Identifier = new byte[] { 0x49, 0x44, 0x33 };
        public int MpegStart { get; private set; }
        public int MpegEnd { get; private set; }
        public Tag Tag { get; private set; }
        public TagHeader OriginalTagHeader { get; private set; }
        public bool HasId3V2 { get; private set; }
        public string OriginalFile { get; private set; }

        private MP3File()
        {
        }

        public static async Task<MP3File> ReadMP3File(string fileName)
        {
            FileStream stream = File.OpenRead(fileName);
            MP3File file = await ReadMP3File(stream, fileName);
            stream.Close();
            return file;
        }

        public static async Task<MP3File> ReadMP3File(Stream stream, string fileName)
        {
            MP3File file = new MP3File() { OriginalFile = fileName };
            
            var fileHeader = new byte[3];
            await stream.ReadAsync(fileHeader, 0, fileHeader.Length);
            file.HasId3V2 = fileHeader.SequenceEqual(Id3V2Identifier);
            if (file.HasId3V2)
            {
                file.OriginalTagHeader = await TagHeader.ReadTagHeader(stream);
                stream.Seek(file.OriginalTagHeader.TagLength + 10, SeekOrigin.Begin);

                while (stream.Position < stream.Length)
                {
                    if (stream.ReadByte() == 0xFF)
                    {
                        if (stream.ReadByte() >= 0xE0)
                        {
                            stream.Seek(-2, SeekOrigin.Current);
                            break;
                        }
                    }
                }

                // Let's find out where the MPEG frames start
                file.MpegStart = (int)stream.Position;

                // Let's start reading some tags, so let's seek to first frame and read all of the frames into an array to parse
                stream.Seek(10, SeekOrigin.Begin);

                switch (file.OriginalTagHeader.Version)
                {
                    case TagVersion.V22:
                        break;
                    case TagVersion.V23:
                        file.Tag = await Tag23Factory.FromStream(stream, file.OriginalTagHeader.TagLength);
                        break;
                    case TagVersion.V24:
                        //Tag = Tag24Factory.FromBytes(tagBytes);
                        break;
                    case TagVersion.Unknown:
                        break;
                }
            }
            else // No ID3v2 Tag so the Mpeg Frames presumably start at the beginning but let's make sure though
            {
                stream.Seek(0, SeekOrigin.Begin);
                bool mpegFramesStartFound = false;
                while (!mpegFramesStartFound)
                {
                    if (stream.ReadByte() == 0xFF)
                    {
                        if (stream.ReadByte() >= 0xE0)
                        {
                            mpegFramesStartFound = true;
                            stream.Seek(-2, SeekOrigin.Current);
                        }
                    }
                }

                // Let's find out where the MPEG frames start
                file.MpegStart = (int)stream.Position;
            }

            //Let's see if this file has an ID3v1 tag
            stream.Seek(-128, SeekOrigin.End);
            if (stream.ReadByte() == 0x54 && stream.ReadByte() == 0x41 && stream.ReadByte() == 0x47)
            {
                // yup, the MPEG frames end at File.Length - 128 bytes
                // we've read 3 bytes to see if the ID3v1 tag is present so the MPEG frames ended 3 bytes ago
                file.MpegEnd = (int)stream.Position - 3;
            }
            else
            {
                // nope.. no ID3v1 tag is present so the MPEG frames end at the end of the file
                file.MpegEnd = (int)stream.Length;
            }

            return file;
        }

        public void SaveClean(string fileName)
        {
            int startRead = -1;
            int endRead = -1;

            startRead = MpegStart;
            endRead = MpegEnd;

            byte[] mpegFrames = new byte[endRead - startRead];

            using (FileStream file = File.OpenRead(this.OriginalFile))
            {
                file.Seek(MpegStart, SeekOrigin.Begin);
                file.Read(mpegFrames, 0, mpegFrames.Length);
            }

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            using (var file = File.Open(fileName, FileMode.CreateNew, FileAccess.Write))
            {
                file.Write(mpegFrames, 0, mpegFrames.Length);
            }
        }

        public async Task SaveAs(string fileName, bool keepV1Tag, TagVersion version)
        {
            int startRead = -1;
            int endRead = -1;
            if (keepV1Tag)
            {
                FileInfo fileInfo = new FileInfo(this.OriginalFile);
                startRead = MpegStart;
                endRead = (int)fileInfo.Length;
            }
            else
            {
                startRead = MpegStart;
                endRead = MpegEnd;
            }

            using (var memoryStream = new MemoryStream())
            {
                await Tag.Write(memoryStream, version);

                if (memoryStream.Length <= MpegStart && MpegEnd == endRead)
                {
                    //we can write the new Tags to the old Tagspace, which is much quicker
                    using (var file = File.Open(fileName, FileMode.Open, FileAccess.Write))
                    {
                        await memoryStream.CopyToAsync(file);
                    }
                }
                else
                {
                    using (var file = File.OpenRead(this.OriginalFile))
                    {
                        file.Seek(MpegStart, SeekOrigin.Begin);
                        var buffer = new byte[4096];
                        while(file.Position < MpegEnd)
                        {
                            int bytesRead = await file.ReadAsync(buffer, 0, (int)Math.Min(MpegEnd - file.Position, buffer.Length));
                            await memoryStream.WriteAsync(buffer, 0, bytesRead);
                        }
                    }

                    if (File.Exists(fileName))
                    {
                        File.Delete(fileName);
                    }

                    using (var file = File.Open(fileName, FileMode.CreateNew, FileAccess.Write))
                    {
                        await memoryStream.CopyToAsync(file);
                    }
                }
            }
        }

        public void Save(bool keepV1Tag, TagVersion version)
        {
            SaveAs(this.OriginalFile, keepV1Tag, version);
        }

        public void Save(TagVersion version)
        {
            SaveAs(this.OriginalFile, false, version);
        }

        public void Save()
        {
            SaveAs(this.OriginalFile, false, TagVersion.V23);
        }
    }
}