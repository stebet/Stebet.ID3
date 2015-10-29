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
                var items = files.Select(x => MP3File.ReadMP3FileAsync(x).Result).ToList();
                Console.WriteLine($"Parsed {files.Length} files in {timer.ElapsedMilliseconds} ms.");
            }
        }
    }
}