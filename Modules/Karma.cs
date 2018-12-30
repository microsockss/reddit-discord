using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DiscordBot;

namespace DiscordBot.Modules
{
    public class Async : ModuleBase<SocketCommandContext>
    {
        [Command("karma")]
        public async Task KarmaAsync()
        {
            var user = Context.User;
            string commandUserId = Context.User.Id.ToString();
            int index = 0;

            // Search for user
            for (int i = 1; i < Program.ARRAY_SIZE; i++)
            {
                string tempUserId = Program.user[i].UserId;
                if (tempUserId == commandUserId)
                {
                    await Console.Out.WriteLineAsync("User found");
                    index = i;
                    break;
                }
                else if (i == Program.ARRAY_SIZE - 1)
                {
                    if (Program.totalUsers != 0 || Program.totalUsers != 1)
                    {
                        Program.totalUsers++;
                    }
                    Program.user[Program.totalUsers].UserId = commandUserId;
                    await Console.Out.WriteLineAsync("User not found, user " + commandUserId + " created as user " + Program.totalUsers);
                    if (Program.totalUsers == 0)
                    {
                        Program.totalUsers = 1;
                    }
                    index = i;
                    break;
                }
            }
            await ReplyAsync(user + " has " + Program.user[index].Karma + " karma!");
        }
    }
}
