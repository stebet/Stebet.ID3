using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace StebetTagger.Core
{
    public static class LyricsHelper
    {
        public static string GetLyrics(string artist, string song)
        {
            string lyricsUrl = "http://lyricwiki.org/api.php?artist={0}&song={1}&fmt=text";
            System.Net.WebClient client = new System.Net.WebClient();
            client.Proxy.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
            string lyricsUrlEncoded = string.Format(System.Globalization.CultureInfo.CurrentCulture, lyricsUrl, HttpUtility.UrlEncode(artist), HttpUtility.UrlEncode(song));
            client.Encoding = System.Text.Encoding.UTF8;
            string lyrics = client.DownloadString(lyricsUrlEncoded).Replace("\r\n", "\n").Replace("\n", "\r\n");

            if (lyrics.Length > 20)
            {
                lyrics = HttpUtility.HtmlDecode(lyrics);
                //lyrics = lyrics.Replace("\r\n", "\n").Replace("\n", "\r\n");
                return lyrics;
            }
            else
            {
                return null;
            }
        }
    }
}
