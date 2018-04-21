using RestApi;

namespace RedditApi.BodiesIn
{
    public class Login : JSONable
    {
        public Login(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        public string username { get; private set; }
        public string password { get; private set; }
    }
}
