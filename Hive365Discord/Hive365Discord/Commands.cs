using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Discord;
using Discord.Audio;
using Discord.Commands;

public class Commands : ModuleBase
{
    [Command("play", RunMode = RunMode.Async), Summary("Makes the Hive365 bot join the voice channel and play")]
    [Alias("join")]
    public async Task Play()
    {
        var channel = (Context.User as IGuildUser)?.VoiceChannel;
        if (channel == null)
        {
            await ReplyAsync("You must be in a voice channel!");
            return;
        }
        
        Hive365Discord.Program.audioClient = await channel.ConnectAsync();
        await SendAsync(Hive365Discord.Program.audioClient, "http://stream.hive365.co.uk:8088/live");
    }

    [Command("stop"), Summary("Makes the Hive365 bot leave the voice channel and stop")]
    [Alias("leave")]
    public async Task Stop()
    {
        await Hive365Discord.Program.audioClient?.StopAsync();
    }

    [Command("song"), Summary("Gets current song")]
    public async Task GetSong()
    {
        string song = await Hive365SocketInfo.GetSong();
        await ReplyAsync($"Current song: **{song}**");
    }

    [Command("dj"), Summary("Gets current DJ")]
    public async Task GetDj()
    {
        string dj = await Hive365SocketInfo.GetDj();
        await ReplyAsync($"Current DJ: **{dj}**");
    }

    [Command("poon"), Summary("Poon the current song")]
    public async Task Poon()
    {
        await Hive365SocketInfo.Poon(Context.User.Username);
        await ReplyAsync($"{Context.User.Mention} thinks that **{await Hive365SocketInfo.GetSong()}** is a bit of a naff Poon!");
    }

    [Command("choon"), Summary("Choon the current song")]
    public async Task Choon()
    {
        await Hive365SocketInfo.Choon(Context.User.Username);
        await ReplyAsync($"{Context.User.Mention} thinks that **{await Hive365SocketInfo.GetSong()}** is a banging Choon!");
    }

    [Command("request"), Summary("Request a song")]
    public async Task Request([Remainder] string song)
    {
        await Hive365SocketInfo.Request(Context.User.Username, song);
        await ReplyAsync($"{Context.User.Mention} has requested **{song}**!");
    }

    [Command("shoutout"), Summary("Make a shoutout")]
    public async Task Shoutout([Remainder] string shoutout)
    {
        await Hive365SocketInfo.Shoutout(Context.User.Username, shoutout);
    }

    [Command("djftw"), Summary("Make a DJFTW")]
    public async Task DjFtw([Remainder] string msg)
    {
        await Hive365SocketInfo.DjFtw(Context.User.Username, msg);
    }

    private Process CreateStream(string path)
    {
        var ffmpeg = new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = $"-i {path} -ac 2 -f s16le -ar 48000 pipe:1",
            UseShellExecute = false,
            RedirectStandardOutput = true,
        };

        return Process.Start(ffmpeg);
    }

    private async Task SendAsync(IAudioClient audioClient, string path)
    {
        var ffmpeg = CreateStream(path);
        var output = ffmpeg.StandardOutput.BaseStream;
        var discordVoiceStream = audioClient.CreatePCMStream(AudioApplication.Music);
        await output.CopyToAsync(discordVoiceStream);
        await discordVoiceStream.FlushAsync();
    }
}
