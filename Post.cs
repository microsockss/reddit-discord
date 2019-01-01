using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot
{
    [Serializable]
    class Post
    {
        private int upvotes;
        private int downvotes;
        private int karma;

        public int Upvotes
        {
            get { return upvotes; }
            set { upvotes = value; }
        }

        public int Downvotes
        {
            get { return downvotes; }
            set { downvotes = value; }
        }

        public int Karma
        {
            get { return karma; }
            set { karma = value; }
        }

        public Post()
        {
            upvotes = 0;
            downvotes = 0;
            karma = 0;
        }

    }
}
