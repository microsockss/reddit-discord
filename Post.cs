using System;
using System.Collections.Generic;
using System.Text;

namespace User_Management_System
{
    [Serializable]
    class Post
    {
        private int upvotes;
        private int downvotes;
        private int karma;
        private long messageid;

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

        public long MessageId
        {
            get { return messageid; }
            set { messageid = value; }
        }

        public Post()
        {
            messageid = 0;
            upvotes = 0;
            downvotes = 0;
            karma = 0;
        }

    }
}
