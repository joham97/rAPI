using rAPI.Answers;

namespace rAPI.DTO
{
    public class Registration : JSONable
    {
        public Registration(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        public string username { get; private set; }
        public string password { get; private set; }
    }
}
