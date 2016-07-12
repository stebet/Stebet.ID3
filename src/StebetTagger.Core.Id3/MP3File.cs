using System;
using System.Buffers;
using System.IO;
using System.Threading.Tasks;

namespace StebetTagger.Core.Id3
{
    public class MP3File
    {
        private MP3File()
        {
        }

        public int MpegStart { get; private set; }
        public int MpegEnd { get; private set; }
        public Tag Tag { get; private set; }
        public TagHeader OriginalTagHeader { get; private set; }
        public bool HasId3V2 { get; private set; }
        public string OriginalFile { get; set; }

        public static async Task<MP3File> ReadMP3FileAsync(string fileName)
        {
            using (var fileStream = File.OpenRead(fileName))
            {
                MP3File file = await ReadMP3FileAsync(fileStream).ConfigureAwait(false);
                file.OriginalFile = fileName;
                return file;
            }
        }

        public static async Task<MP3File> ReadMP3FileAsync(Stream stream)
        {
            var file = new MP3File();
            file.HasId3V2 = (stream.ReadByte() == Constants.ID3Header[0] && stream.ReadByte() == Constants.ID3Header[1] && stream.ReadByte() == Constants.ID3Header[2]);
            if (file.HasId3V2)
            {
                // Let's read the tag header.
                file.OriginalTagHeader = await TagHeader.ReadTagHeader(stream).ConfigureAwait(false);

                // Let's read the tag frames into a byte array and wrap it into a memorystream.
                var tags = ArrayPool<byte>.Shared.Rent(file.OriginalTagHeader.TagLength);
                await stream.ReadAsync(tags, 0, file.OriginalTagHeader.TagLength).ConfigureAwait(false);
                using (var memoryStream = new MemoryStream(tags, 0, file.OriginalTagHeader.TagLength))
                {
                    // Let's read the tag.
                    switch (file.OriginalTagHeader.Version)
                    {
                        case TagVersion.V22:
                            break;
                        case TagVersion.V23:
                            file.Tag = await Tag23Factory.FromStream(memoryStream, file.OriginalTagHeader.TagLength).ConfigureAwait(false);
                            break;
                        case TagVersion.V24:
                            //Tag = Tag24Factory.FromBytes(tagBytes);
                            break;
                        case TagVersion.Unknown:
                            break;
                    }
                }
                ArrayPool<byte>.Shared.Return(tags);
            }
            else // No ID3v2 Tag so the Mpeg Frames presumably start at the beginning but let's make sure though
            {
                stream.Seek(0, SeekOrigin.Begin);
            }

            // Now let's find the MPEG frames, seek until we find the first 0xFF byte that
            // should be part of the first Mpeg frame.
            while (stream.Position < stream.Length && stream.ReadByte() != 0xFF)
            {
                if (stream.ReadByte() >= 0xE0)
                {
                    stream.Seek(-2, SeekOrigin.Current);
                    break;
                }
            }

            // We should be at the first MPEG frame
            file.MpegStart = (int)stream.Position;

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

        public async Task SaveClean(string fileName)
        {
            int startRead = MpegStart;
            int endRead = MpegEnd;

            var mpegFrames = new byte[endRead - startRead];

            using (var file = File.OpenRead(OriginalFile))
            {
                file.Seek(MpegStart, SeekOrigin.Begin);
                await file.ReadAsync(mpegFrames, 0, mpegFrames.Length).ConfigureAwait(false);
            }

            using (var file = File.Open(fileName, FileMode.Truncate, FileAccess.Write))
            {
                await file.WriteAsync(mpegFrames, 0, mpegFrames.Length).ConfigureAwait(false);
            }
        }

        public async Task SaveAsAsync(string fileName, bool keepV1Tag, TagVersion version)
        {
            int startRead = -1;
            int endRead = -1;
            if (keepV1Tag)
            {
                FileInfo fileInfo = new FileInfo(OriginalFile);
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
                await Tag.Write(memoryStream, version).ConfigureAwait(false);

                if (memoryStream.Length <= MpegStart && MpegEnd == endRead)
                {
                    //we can write the new Tags to the old Tagspace, which is much quicker
                    using (var file = File.Open(fileName, FileMode.Open, FileAccess.Write))
                    {
                        await memoryStream.CopyToAsync(file).ConfigureAwait(false);
                    }
                }
                else
                {
                    using (var file = File.OpenRead(OriginalFile))
                    {
                        file.Seek(MpegStart, SeekOrigin.Begin);
                        var buffer = new byte[4096];
                        while (file.Position < MpegEnd)
                        {
                            int bytesRead = await file.ReadAsync(buffer, 0, (int)Math.Min(MpegEnd - file.Position, buffer.Length)).ConfigureAwait(false);
                            await memoryStream.WriteAsync(buffer, 0, bytesRead).ConfigureAwait(false);
                        }
                    }

                    if (File.Exists(fileName))
                    {
                        File.Delete(fileName);
                    }

                    using (var file = File.Open(fileName, FileMode.CreateNew, FileAccess.Write))
                    {
                        await memoryStream.CopyToAsync(file).ConfigureAwait(false);
                    }
                }
            }
        }

        public Task SaveAsync(bool keepV1Tag = false, TagVersion version = TagVersion.V23) => SaveAsAsync(OriginalFile, keepV1Tag, version);
    }
}