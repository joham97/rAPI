namespace RedditApi.BodiesOut
{
    public class UserData : DataAnswer
    {
        public UserData(int userId, string username)
        {
            this.userId = userId;
            this.username = username;
        }

        public int userId { get; private set; }
        public string username { get; set; }
    }
}
