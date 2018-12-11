using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot
{
    class Program
    {
        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();
        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        CommandService commands;

        public async Task RunBotAsync()
        {

            DiscordSocketConfig _config = new DiscordSocketConfig();
            _config.MessageCacheSize = 1;

            _client = new DiscordSocketClient(_config);
            _commands = new CommandService();

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            string botToken = "TOKEN";

            // event subscription
            _client.Log += Log;

            await RegisterCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, botToken);

            await _client.StartAsync();

            await Task.Delay(-1);

        }

        private Task Log(LogMessage arg)
        {
            Console.WriteLine(arg);

            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        public async Task RegisterVote()
        {
            _client.ReactionAdded += HandleVoteAsync;

            // await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task HandleVoteAsync(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            var reactions = arg3.Emote;

            var user = arg3.UserId;

            await Console.Out.WriteLineAsync("THIS IS THE USER THAT POSTED: " + user);

            await Console.Out.WriteLineAsync("THIS IS THE REACTIONS VARIABLE: " + reactions);

            throw new NotImplementedException();
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;

            if (message == null || message.Author.IsBot) return;
                                
            int argPos = 0;

            if (message.HasStringPrefix(".", ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var context = new SocketCommandContext(_client, message);

                var result = await _commands.ExecuteAsync(context, argPos, _services);

                SocketGuild guild = ((SocketGuildChannel)message.Channel).Guild;

                IEmote upvote = guild.Emotes.First(e => e.Name == "upvote");
                await message.AddReactionAsync(upvote);
                await Console.Out.WriteLineAsync("bot upvoted");

                IEmote downvote = guild.Emotes.First(e => e.Name == "downvote");
                await message.AddReactionAsync(downvote);
                await Console.Out.WriteLineAsync("bot downvoted");

                if (!result.IsSuccess)
                    Console.WriteLine(result.ErrorReason);
            }
        }

    }
}
