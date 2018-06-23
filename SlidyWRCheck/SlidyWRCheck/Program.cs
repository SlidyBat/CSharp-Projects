using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace SlidyWRCheck
{
    public class Program
    {
        private DiscordSocketClient client;
        private CommandService commands;
        private IServiceProvider services;

        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            client = new DiscordSocketClient();
            client.Log += Log;

            string token = "***REMOVED***";
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            Sql.Init();

            services = new ServiceCollection().BuildServiceProvider();
            commands = new CommandService();
            await InstallCommands();

            await Task.Delay(-1);
        }

        private async Task InstallCommands()
        {
            client.MessageReceived += HandleCommand;
            await commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task HandleCommand(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;
            if(message == null)
            {
                return;
            }

            int argPos = 0;
            if(!(message.HasCharPrefix('%', ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos)))
            {
                return;
            }

            var context = new CommandContext(client, message);
            var result = await commands.ExecuteAsync(context, argPos, services);
            if(!result.IsSuccess)
            {
                await context.Channel.SendMessageAsync(result.ErrorReason);
            }
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
