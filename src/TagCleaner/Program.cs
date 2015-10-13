using System;
using System.Diagnostics;
using System.IO;
using StebetTagger.Core.Id3;

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
                }
                Console.WriteLine($"Parsed {files.Length} files in {timer.ElapsedMilliseconds} ms.");
            }
        }
    }
}