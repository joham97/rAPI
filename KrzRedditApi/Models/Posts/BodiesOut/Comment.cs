using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditApi.BodiesOut
{
    public class Comment : DataAnswer
    {

        public Comment(int id, string text, string user, int upvotes, int downvotes)
        {
            this.id = id;
            this.text = text;
            this.user = user;
            this.upvotes = upvotes;
            this.downvotes = downvotes;
            this.comment = new List<Comment>();
        }

        public Comment(int id, string text, string user, int upvotes, int downvotes, int yourvote) : this(id, text, user, upvotes, downvotes)
        {
            this.yourvote = yourvote;
        }

        public int id { get; private set; }
        public string text { get; private set; }
        public string user { get; private set; }
        public int upvotes { get; private set; }
        public int downvotes { get; private set; }
        public int yourvote { get; private set; }
        public List<Comment> comment;
    }
}
