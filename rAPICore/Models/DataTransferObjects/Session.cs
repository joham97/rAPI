using rAPI.Answers;

namespace rAPI.DTO
{
    public class Session : JSONable
    {
        public Session(int userid, long lastactivity)
        {
            this.userid = userid;
            this.lastactivity = lastactivity;
        }

        public int userid { get; private set; }
        public long lastactivity { get; private set; }
    }
}