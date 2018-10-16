using rAPI.Answers;
using System.Collections.Generic;

namespace rAPI.DTO
{
    public class Comment : DataAnswer
    {

        public Comment(int commentId, string text, string user, int upvotes, int downvotes)
        {
            this.commentId = commentId;
            this.text = text;
            this.user = user;
            this.upvotes = upvotes;
            this.downvotes = downvotes;
            this.comment = new List<Comment>();
        }

        public Comment(int commentId, string text, string user, int upvotes, int downvotes, int yourvote) : this(commentId, text, user, upvotes, downvotes)
        {
            this.yourvote = yourvote;
        }

        public int commentId { get; set; }
        public string text { get; private set; }
        public string user { get; private set; }
        public int upvotes { get; private set; }
        public int downvotes { get; private set; }
        public int yourvote { get; private set; }
        public List<Comment> comment;
    }
}
