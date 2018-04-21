namespace RedditApi.BodiesOut
{
    public class LoginData : DataAnswer
    {
        public LoginData(string sessionkey, int userId)
        {
            this.sessionkey = sessionkey;
            this.userId = userId;
        }

        public string sessionkey { get; set; }
        public int userId { get; private set; }
    }
}
