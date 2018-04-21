using RestApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditApi.BodiesIn
{
    public class Comment : JSONable
    {
        public Comment(string text, int postId, int commentId)
        {
            this.text = text;
            this.postId = postId;
            this.commentId = commentId;
        }
        
        public string text { get; private set; }
        public int postId { get; private set; }
        public int commentId { get; private set; }
    }
}
