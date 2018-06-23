using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Discord;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

enum Style
{
    Invalid = -1,
    Normal,
    Legit,
    WOnly,
    AOnly,
    DOnly,
    Sideways,
    HSW,
    NoLimit,
    LowGravity,
    SlowMotion,
    Vel400,
    HighFOV,
    LowFOV,
    Segmenting,
    TAS,
    TASLG,
    TagTeam
}

class SlidyRecord
{
    private static Dictionary<string, Style> styles = null;

    public string PlayerName { get; }
    private int SteamAccountId { get; }
    public Style RecordStyle { get; }
    public float Time { get; }
    public int Strafes { get; }
    public int Jumps { get; }
    public int SSJ { get; }
    public float Sync { get; }
    public float StrafeTime { get; }

    public SlidyRecord(string playerName_in, float time_in, int strafes_in, int jumps_in, int ssj_in, float sync_in, float strafetime_in)
    {
        PlayerName = playerName_in;
        Time = time_in;
        Strafes = strafes_in;
        Jumps = jumps_in;
        SSJ = ssj_in;
        Sync = sync_in;
        StrafeTime = strafetime_in;
    }

    public SlidyRecord(MySqlDataReader reader)
    {
        PlayerName = reader["lastname"].ToString();
        SteamAccountId = reader.GetInt32("steamaccountid");
        RecordStyle = (Style)reader.GetInt32("style");
        Time = reader.GetFloat("time");
        Strafes = reader.GetInt32("strafes");
        Jumps = reader.GetInt32("jumps");
        SSJ = reader.GetInt32("ssj");
        Sync = reader.GetFloat("sync");
        StrafeTime = reader.GetFloat("strafetime");
    }

    public async Task<EmbedBuilder> GetEmbed(string title)
    {
        long steam64 = SteamAccountId + 76561197960265728;
        string steamurl = "http://www.steamcommunity.com/profiles/" + steam64.ToString();

        var steamuserapi = "http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=49B6F0BCFC5E2F6470822764DF6A0BDA&steamids=";
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(steamuserapi + steam64.ToString());
        string response;

        using (var webresponse = await request.GetResponseAsync())
        using (var stream = webresponse.GetResponseStream())
        using (var reader = new StreamReader(stream))
        {
            response = await reader.ReadToEndAsync();
        }

        dynamic objects = JsonConvert.DeserializeObject(response);
        string avatar = objects.response.players[0].avatarfull;

        EmbedBuilder builder = new EmbedBuilder();

        builder.WithTitle("**" + title + " - " + GetStyleName(RecordStyle) + "**");
        builder.Color = new Color(255, 128, 0);
        builder.ThumbnailUrl = avatar;

        builder.AddField("Player", "[" + PlayerName + "]" + "(" + steamurl + ")", true);
        builder.AddField("Time", FormatTime(Time), true);

        string stats = String.Format("**Strafes**: {0}\t\t\t\t\t\t**Sync**: {1:0.00}%\t\t\t\t\t\t**Jumps**: {2}\n**SSJ**: {3}\t\t\t\t\t\t**Strafe Time**: {4:0.00}",
            Strafes,
            Sync,
            Jumps,
            SSJ,
            StrafeTime);
        builder.AddField("Stats", stats);

        return builder;
    }

    public static Style GetStyleFromName(string styleName)
    {
        if (styles == null)
        {
            styles = new Dictionary<string, Style>();

            styles.Add("au", Style.Normal);
            styles.Add("auto", Style.Normal);
            styles.Add("n", Style.Normal);
            styles.Add("normal", Style.Normal);

            styles.Add("l", Style.Legit);
            styles.Add("legit", Style.Legit);

            styles.Add("w", Style.WOnly);
            styles.Add("wonly", Style.WOnly);
            styles.Add("w-only", Style.WOnly);

            styles.Add("a", Style.AOnly);
            styles.Add("aonly", Style.AOnly);
            styles.Add("a-only", Style.AOnly);

            styles.Add("d", Style.DOnly);
            styles.Add("donly", Style.DOnly);
            styles.Add("d-only", Style.DOnly);

            styles.Add("sw", Style.Sideways);
            styles.Add("sideways", Style.Sideways);

            styles.Add("hsw", Style.HSW);
            styles.Add("halfsideways", Style.HSW);
            styles.Add("half-sideways", Style.HSW);

            styles.Add("nl", Style.NoLimit);
            styles.Add("nolimit", Style.NoLimit);

            styles.Add("slow", Style.SlowMotion);
            styles.Add("slomo", Style.SlowMotion);
            styles.Add("slowmo", Style.SlowMotion);
            styles.Add("slowmotion", Style.SlowMotion);

            styles.Add("400", Style.Vel400);
            styles.Add("400vel", Style.Vel400);
            styles.Add("400legit", Style.Vel400);

            styles.Add("hifov", Style.HighFOV);
            styles.Add("hfov", Style.HighFOV);
            styles.Add("highfov", Style.HighFOV);
            styles.Add("hi", Style.HighFOV);

            styles.Add("lofov", Style.LowFOV);
            styles.Add("lfov", Style.LowFOV);
            styles.Add("lowfov", Style.LowFOV);
            styles.Add("low", Style.LowFOV);

            styles.Add("seg", Style.Segmenting);
            styles.Add("segmented", Style.Segmenting);
            styles.Add("segmenting", Style.Segmenting);

            styles.Add("tas", Style.TAS);

            styles.Add("taslg", Style.TASLG);
            styles.Add("lgtas", Style.TASLG);

            styles.Add("tagteam", Style.TagTeam);
        }

        Style style;
        if(!styles.TryGetValue(styleName, out style))
        {
            style = Style.Invalid;
        }

        return style;
    }

    public static string GetStyleName(Style style)
    {
        switch(style)
        {
            case Style.Normal:
                return "Auto";
            case Style.Legit:
                return "Legit";
            case Style.WOnly:
                return "W-Only";
            case Style.AOnly:
                return "A-Only";
            case Style.DOnly:
                return "D-Only";
            case Style.Sideways:
                return "Sideways";
            case Style.HSW:
                return "Half-Sideways";
            case Style.NoLimit:
                return "Auto [No Limit]";
            case Style.LowGravity:
                return "Low Gravity";
            case Style.SlowMotion:
                return "Slowmotion";
            case Style.Vel400:
                return "400 Vel";
            case Style.HighFOV:
                return "High FOV";
            case Style.LowFOV:
                return "Low FOV";
            case Style.Segmenting:
                return "Segmenting";
            case Style.TAS:
                return "TAS";
            case Style.TASLG:
                return "TAS Low Gravity";
            case Style.TagTeam:
                return "Tag-Team Relay";
            default:
                return "Invalid Style";
        }
    }

    private static string FormatTime(float time)
    {
        int iTime = (int)Math.Round(time * 1000);

        int mins = 0;
        int secs = 0;
        int milli = iTime;

        if (milli < 0)
            milli = -milli;

        if (milli >= 60000)
        {
            mins = (int)(milli / 60000.0);
            milli %= 60000;
        }

        if (milli >= 1000)
        {
            secs = (int)(milli / 1000.0);
            milli %= 1000;
        }

        string timeString = "";
        if(time < 0)
        {
            timeString = "-";
        }

        timeString += String.Format("{0:D2}:{1:D2}.{2:D3}s", mins, secs, milli);

        return timeString;
    }
}
