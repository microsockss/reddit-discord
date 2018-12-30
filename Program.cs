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
        public static int totalUsers = 0;
        public const int ARRAY_SIZE = 1000;
        public static int index = 0;
        public static User[] user = new User[ARRAY_SIZE];

        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();
        private SocketGuild _guild;
        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        CommandService commands;

        Image upvote = new Image("Resources/upvote.png");
        Image downvote = new Image("Resources/downvote.png");

        public Program()
        {
        }

        public async Task RunBotAsync()
        {
            //Initialize2();
            //await Save();
            //await Console.Out.WriteLineAsync("Created blank dataset.");
            await Load();
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

            await Initialize();

            await RegisterVote();

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

        public async Task Initialize()
        {
            _client.JoinedGuild += UploadEmote;
        }

        private async Task UploadEmote(SocketGuild guild)
        {
            await guild.CreateEmoteAsync("upvote", upvote);
            await Console.Out.WriteLineAsync("UPLOADED UPVOTE EMOJI");
            await guild.CreateEmoteAsync("downvote", downvote);
            await Console.Out.WriteLineAsync("UPLOADED DOWNVOTE EMOJI");
            throw new NotImplementedException();
        }

        public async Task RegisterVote()
        {
            _client.ReactionAdded += HandleVoteAsync;

            _client.ReactionRemoved += HandleVoteAsync;

            // await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task HandleVoteAsync(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            var reaction = arg3.Emote;

            var message = await arg1.DownloadAsync();

            SocketGuild guild = ((SocketGuildChannel)message.Channel).Guild;

            IEmote upvote = guild.Emotes.First(e => e.Name == "upvote");
            IEmote downvote = guild.Emotes.First(e => e.Name == "downvote");

            var upvotes = message.Reactions[upvote].ReactionCount;
            var downvotes = message.Reactions[downvote].ReactionCount;

            var author = message.Author.Id.ToString();
            
            var voter = arg3.UserId;

            await Console.Out.WriteLineAsync("Author ID: " + author);

            await Console.Out.WriteLineAsync("Voter ID: " + voter);

            await Console.Out.WriteLineAsync("Total Upvotes: " + upvotes);

            await Console.Out.WriteLineAsync("Total Downvotes: " + downvotes);

            int karma = upvotes - downvotes;

            if (karma == 0) return;

            // Search for user
            for (int i = 1; i < ARRAY_SIZE; i++)
            {
                string tempUserId = user[i].UserId;
                if (tempUserId == author)
                {
                    await Console.Out.WriteLineAsync("User found");
                    index = i;
                    break;
                }
                else if (i == ARRAY_SIZE - 1)
                {
                    if (totalUsers != 0 || totalUsers != 1)
                    {
                        totalUsers++;
                    }
                    user[totalUsers].UserId = author;
                    index = 1;
                    await Console.Out.WriteLineAsync("User not found, user " + author + " created as user " + totalUsers);
                    if (totalUsers == 0)
                    {
                        totalUsers = 1;
                    }
                    break;
                }
            }

            user[index].Karma += karma;

            await Save();
            Console.WriteLine("Granted karma and saved to database.");

            throw new NotImplementedException();
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            if (message == null || message.Author.IsBot) return;
                                
            int argPos = 0;

            if (message.HasStringPrefix("?", ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var context = new SocketCommandContext(_client, message);

                var result = await _commands.ExecuteAsync(context, argPos, _services);

                if (!result.IsSuccess)
                    Console.WriteLine(result.ErrorReason);
            }


            if (message.HasStringPrefix(".", ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                SocketGuild guild = ((SocketGuildChannel)message.Channel).Guild;

                IEmote upvote = guild.Emotes.First(e => e.Name == "upvote");
                await message.AddReactionAsync(upvote);
                await Console.Out.WriteLineAsync("bot upvoted");

                IEmote downvote = guild.Emotes.First(e => e.Name == "downvote");
                await message.AddReactionAsync(downvote);
                await Console.Out.WriteLineAsync("bot downvoted");
            }
        }

        public async Task Save()
        {
            List<User> users = new List<User>();
            for (int i = 0; i < user.Length; i++)
            {
                users.Add(user[i]);
            }

            Serialize.WriteToBinaryFile("users.bin", users, false);
            Serialize.WriteToBinaryFile("totalusers.bin", totalUsers, false);
        }

        public async Task Load()
        {
            List<User> users = new List<User>();
            users = Serialize.ReadFromBinaryFile<List<User>>("users.bin");
            totalUsers = Serialize.ReadFromBinaryFile<int>("totalusers.bin");

            for (int i = 0; i < users.Count; i++)
            {
                user[i] = users[i];
            }
        }

        public static void Initialize2()
        {
            for (int i = 0; i < user.Length; i++)
            {
                user[i] = new User();
            }
        }
    }
}
