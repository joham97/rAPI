using rAPI.Answers;

namespace rAPI.DTO
{
    public class CommentVote : JSONable
    {
        public CommentVote(int commentId, int value, int userId)
        {
            this.commentId = commentId;
            this.value = value;
            this.userId = userId;
        }
        
        public int commentId { get; private set; }
        public int value { get; private set; }
        public int userId { get; set; }
    }
}
