using System;
using System.Text;

namespace DiscordBot
{
    [Serializable]
    class User
    {
        private string userId;
        private int karma;

        public string UserId
        {
            get { return userId; }
            set { userId = value; }
        }

        public int Karma
        {
            get { return karma; }
            set { karma = value; }
        }

        public User()
        {
            userId = null;
            karma = 0;
        }

    }
}
