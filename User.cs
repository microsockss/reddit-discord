using System;
using System.Text;

namespace User_Management_System
{
    [Serializable]
    class User
    {
        private string userId;
        private int karma;
        private int totalPosts;

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

        public int TotalPosts
        {
            get { return totalPosts; }
            set { totalPosts = value; }
        }

        public User()
        {
            userId = null;
            karma = 0;
            totalPosts = 0;
        }

    }
}
