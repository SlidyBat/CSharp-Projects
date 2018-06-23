using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;

class Hive365SocketInfo
{
    private enum SocketInfo
    {
        Info,
        Request,
        Shoutout,
        Choon,
        Poon,
        DjFtw,
        HeartBeat
    };

    private static readonly HttpClient client = new HttpClient();
    private static string hive365Info = "http://data.hive365.co.uk/stream/info.php";

    private static string Base64Encode(string plainText)
    {
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        return System.Convert.ToBase64String(plainTextBytes);
    }

    public static async Task Request(string name, string song)
    {
        var values = new Dictionary<string, string>
        {
            { "n", Base64Encode(name) },
            { "s", Base64Encode(song) }
        };

        var content = new FormUrlEncodedContent(values);
        var response = await client.PostAsync("http://www.hive365.co.uk/plugin/request.php", content);
    }

    public static async Task Shoutout(string name, string shoutout)
    {
        var values = new Dictionary<string, string>
        {
            { "n", Base64Encode(name) },
            { "s", Base64Encode(shoutout) }
        };

        var content = new FormUrlEncodedContent(values);
        var response = await client.PostAsync("http://www.hive365.co.uk/plugin/shoutout.php", content);
    }

    public static async Task DjFtw(string name, string msg)
    {
        var values = new Dictionary<string, string>
        {
            { "n", Base64Encode(name) },
            { "s", Base64Encode(msg) }
        };

        var content = new FormUrlEncodedContent(values);
        var response = await client.PostAsync("http://www.hive365.co.uk/plugin/djrate.php", content);
    }

    public static async Task Poon(string name)
    {
        var values = new Dictionary<string, string>
        {
            { "n", Base64Encode(name) },
            { "t", "4" }
        };

        var content = new FormUrlEncodedContent(values);
        var response = await client.PostAsync("http://www.hive365.co.uk/plugin/song_rate.php", content);
    }

    public static async Task Choon(string name)
    {
        var values = new Dictionary<string, string>
        {
            { "n", Base64Encode(name) },
            { "t", "3" }
        };

        var content = new FormUrlEncodedContent(values);
        var response = await client.PostAsync("http://www.hive365.co.uk/plugin/song_rate.php", content);
    }

    public static async Task<string> GetSong()
    {
        var response = await client.GetStringAsync(hive365Info);
        dynamic objects = JsonConvert.DeserializeObject(response);
        string song = objects.info.artist_song;
        return WebUtility.HtmlDecode(song);
    }

    public static async Task<string> GetDj()
    {
        var response = await client.GetStringAsync(hive365Info);
        dynamic objects = JsonConvert.DeserializeObject(response);
        string dj = objects.info.title;
        return WebUtility.HtmlDecode(dj);
    }
}


