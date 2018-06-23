using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace DiscordWerdGameSolver
{
    class Program
    {
        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient client;
        private bool enabled = false;
        private List<string> words;
        private string input;

        public async Task MainAsync()
        {
            var file = File.ReadAllLines("sgb-words.txt");
            var fiveWords = new List<string>(file);
            file = File.ReadAllLines("20k.txt");
            var commonWords = new List<string>(file);

            words = fiveWords.Intersect(commonWords).ToList();

            client = new DiscordSocketClient();
            client.MessageReceived += OnMessageReceived;

            string token = "***REMOVED***";
            await client.LoginAsync(TokenType.User, token);
            await client.StartAsync();

            Console.WriteLine("Ready!");

            await Task.Delay(-1);
        }

        private async Task OnMessageReceived(SocketMessage message)
        {
            if (!enabled)
            {
                if (message.Content == "%game")
                {
                    enabled = true;
                    input = "audio";
                    await message.Channel.SendMessageAsync("%guess audio");
                }
            }
            else if (enabled && message.Author.Username == "ChiliBot" && message.Content.Contains("by __SlidyBat__"))
            {
                int score = int.Parse(Regex.Match(message.Content, @"【(?<Score>\d)】").Groups["Score"].Value);
                
                words.RemoveAll(word => ((word == input) || (ScoreMatch(word, input) != score)));
                input = words.Last();

                await message.Channel.SendMessageAsync("%guess " + input);
            }
            else if(enabled && message.Author.Username == "ChiliBot" && message.Content.Contains("__SlidyBat__ guessed the mother"))
            {
                enabled = false;
                await message.Channel.SendMessageAsync("SlidyBot wins!!");
            }
        }

        private List<int> FillBuckets(string word)
        {
            List<int> buckets = new List<int>(new int[26]);

            foreach (char c in word)
            {
                buckets[c - 'a']++;
            }

            return buckets;
        }

        int ScoreMatch(string word1, string word2)
        {
            var buckets1 = FillBuckets(word1);
            var buckets2 = FillBuckets(word2);

            int score = 0;
            for(int i = 0; i < 26; i++)
            {
                score += Math.Min(buckets1[i], buckets2[i]);
            }

            for( int i = 0; i < 5; i++)
            {
                if(word1[i] == word2[i])
                {
                    score++;
                }
            }

            return score;
        }
    }
}
