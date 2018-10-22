using rAPI.Answers;

namespace rAPI.DTO
{
    public class CommentVote : JSONable
    {
        public CommentVote(int commentId, int value)
        {
            this.commentId = commentId;
            this.value = value;
        }
        
        public int commentId { get; private set; }
        public int value { get; private set; }
    }
}
