using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using StebetTagger.Core.Id3;
using StebetTagger.Core.Id3.Tags;

namespace TagCleaner
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                var timer = Stopwatch.StartNew();
                string[] files = Directory.GetFiles(args[0], "*.mp3", SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    var mp3File = MP3File.ReadMP3FileAsync(file).Result;
                    //foreach(var item in mp3File.Tag.Frames.OfType<AttachedPicture>())
                    //{
                    //    using (var imgFile = File.OpenWrite($"{Guid.NewGuid().ToString()}.jpg"))
                    //    {
                    //        imgFile.Write(item.Data, 0, item.Data.Length);
                    //    }
                    //}
                }
                Console.WriteLine($"Parsed {files.Length} files in {timer.ElapsedMilliseconds} ms.");
            }
        }
    }
}