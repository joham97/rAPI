using rAPI.Answers;

namespace rAPI.DTO
{
    public class Vote : JSONable
    {
        public Vote(int postId, int value, int userId)
        {
            this.postId = postId;
            this.value = value;
            this.userId = userId;
        }
        
        public int postId { get; private set; }
        public int value { get; private set; }
        public int userId { get; set; }
    }
}
