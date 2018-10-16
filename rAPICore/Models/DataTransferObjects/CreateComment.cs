using rAPI.Answers;

namespace rAPI.DTO
{
    public class CreateComment : JSONable
    {
        public CreateComment(string text, int postId, int commentId)
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
