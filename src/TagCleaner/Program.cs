using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using StebetTagger.Core;
using StebetTagger.Core.Id3;
using StebetTagger.Core.Id3.Tags;
using System.Globalization;

namespace TagCleaner
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                string[] files = Directory.GetFiles(args[0], "*.mp3", SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    MP3File mp3File = MP3File.ReadMP3File(file).Result;

                    /*
                    for (int i = 0; i < mp3File.Tag.Tags.Count; i++)
                    {
                        if (
                            !(mp3File.Tag.Tags[i] is Album) &&
                            !(mp3File.Tag.Tags[i] is AlbumArtist) &&
                            !(mp3File.Tag.Tags[i] is Artist) &&
                            !(mp3File.Tag.Tags[i] is AttachedPicture) &&
                            !(mp3File.Tag.Tags[i] is Title) &&
                            !(mp3File.Tag.Tags[i] is TrackNumber) &&
                            !(mp3File.Tag.Tags[i] is UnsynchronizedLyrics) &
                            !(mp3File.Tag.Tags[i] is Year)
                            )
                        {
                            mp3File.Tag.Tags.Remove(mp3File.Tag.Tags[i]);
                            i = 0;
                        }
                    }
                    */

                    /*
                    if (mp3File.Tag.Tags.Count(x => x is UnsynchronizedLyrics) == 0)
                    {
                        Console.Write(String.Format(CultureInfo.InvariantCulture, "No lyrics found for {0} - {1}, searching for lyrics...", mp3File.Tag.Artist, mp3File.Tag.Title));
                        string lyrics = LyricsHelper.GetLyrics(mp3File.Tag.Artist, mp3File.Tag.Title);
                        if (!string.IsNullOrEmpty(lyrics))
                        {
                            StebetTagger.Core.Id3.Tags.UnsynchronizedLyrics lyricsTag = null;
                            foreach (Frame frame in mp3File.Tag.Tags)
                            {
                                lyricsTag = frame as UnsynchronizedLyrics;
                                if (lyricsTag != null)
                                {
                                    break;
                                }
                            }
                            if (lyricsTag == null)
                            {
                                lyricsTag = new UnsynchronizedLyrics();
                            }
                            lyricsTag.Language = "eng";
                            lyricsTag.Text = lyrics;

                            mp3File.Tag.Tags.Add(lyricsTag);
                            Console.WriteLine("found.");
                        }
                        else
                        {
                            Console.WriteLine("not found.");
                        }
                    }

                    Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Saving {0}", mp3File.OriginalFile));
                    mp3File.Save(false, TagVersion.V23);
                    */
                    /*
                    string musicFolder = System.Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
                    string artistFolderName = mp3File.Tag.AlbumArtist;
                    string albumFolderName = mp3File.Tag.Album;
                    string illegalChars = "/\\:*?\"<>|";
                    for (int i = 0; i < illegalChars.Length; i++)
                    {
                        artistFolderName = artistFolderName.Replace(illegalChars[i].ToString(), string.Empty);
                        albumFolderName = albumFolderName.Replace(illegalChars[i].ToString(), string.Empty);
                    }
                    string artistFolder = musicFolder + "\\" + artistFolderName;
                    string albumFolder = artistFolder + "\\" + albumFolderName + " [" + mp3File.Tag.Year + "]";
                    
                    
                    if (!Directory.Exists(artistFolder))
                    {
                        Directory.CreateDirectory(artistFolder);
                    }

                    if (!Directory.Exists(albumFolder))
                    {
                        Directory.CreateDirectory(albumFolder);
                    }

                    FileInfo info = new FileInfo(mp3File.OriginalFile);
                    File.Move(mp3File.OriginalFile, albumFolder + "\\" + info.Name);
                    File.SetAttributes(albumFolder + "\\" + info.Name, FileAttributes.ReadOnly);
                    */
                }
            }
        }
    }
}