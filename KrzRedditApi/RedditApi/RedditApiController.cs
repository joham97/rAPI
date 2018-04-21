using RestApi;
using RedditApi.BodiesIn;
using RedditApi.BodiesOut;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditApi
{
    public class RedditApiController : RESTApi
    {

        private string IMAGE_PATH = @"C:\xampp\htdocs";

        private string SUB_PATH = @"\assets\";

        private Dictionary<string, Session> sessions;
        private Database db;

        public RedditApiController(int port) : base(port)
        {
            this.sessions = new Dictionary<string, Session>();
            this.db = new Database("reddit.db");
        }

        protected override string HandleRequest(Request request)
        {
            string answer = null;

            #region routes
            try
            {
                string route;
                if (request.Pfad.Contains("?"))
                    route = request.Pfad.Substring(0, request.Pfad.IndexOf('?'));
                else
                    route = request.Pfad;

                if (request.Data.ContainsKey("sessionkey") &&
                    !this.sessions.ContainsKey(request.Data["sessionkey"]))
                {
                    answer = new NormalAnswer(false, "unauthorized", 401).ToJSON();
                }

                #region Test
                if (route.Equals("/test") && request.Type == RequestType.GET)
                {
                    answer = new NormalAnswer(true, "Test successful", 200).ToJSON();
                }
                #endregion
                #region Session
                else if (route.Equals("/login") && request.Type == RequestType.GET)
                {
                    var returner = db.Login(new BodiesIn.Login(request.Data["username"], request.Data["password"]));
                    if (returner.success)
                    {
                        var loginAnswer = returner as ComplexAnswer;
                        var loginData = loginAnswer.data as LoginData;
                        // Remove Session
                        List<string> removeSessions = new List<string>();
                        foreach (var a in this.sessions)
                        {
                            if (a.Value.userid == loginData.userId)
                            {
                                removeSessions.Add(a.Key);
                            }
                        }
                        foreach (var a in removeSessions)
                        {
                            this.sessions.Remove(a);
                        }
                        // Generate new Sessionkey
                        string key = this.CreateSessionKey();
                        while (this.sessions.ContainsKey(key))
                        {
                            key = this.CreateSessionKey();
                        }
                        this.sessions.Add(key, new Session(loginData.userId, DateTime.Now.Ticks));
                        loginData.sessionkey = key;
                        answer = loginAnswer.ToJSON();
                    }
                    else
                    {
                        answer = returner.ToJSON();
                    }
                }
                else if (route.Equals("/logout") && request.Type == RequestType.GET)
                {
                    this.sessions.Remove(request.Data["sessionkey"]);
                    answer = new NormalAnswer(true, "logout successful", 200).ToJSON();
                }
                else if (route.Equals("/register") && request.Type == RequestType.GET)
                {
                    answer = db.Registrieren(new BodiesIn.Registration(request.Data["username"], request.Data["password"])).ToJSON();
                }
                else if (route.Equals("/validate") && request.Type == RequestType.GET)
                {
                    answer = new NormalAnswer(true, "true", 200).ToJSON();
                }
                else if (route.Equals("/user") && request.Type == RequestType.GET)
                {
                    answer = db.GetUser(Convert.ToInt16(request.Data["userId"])).ToJSON();
                }
                #endregion
                #region Posts
                else if (route.Equals("/post") && request.Type == RequestType.GET)
                {
                    var postId = Convert.ToInt32(request.Data["postId"]);
                    if (request.Data.ContainsKey("sessionkey"))
                    {
                        var userId = sessions[request.Data["sessionkey"]].userid;
                        answer = db.GetSinglePost(postId, userId).ToJSON();
                    }
                    else
                    {
                        answer = db.GetSinglePost(postId).ToJSON();
                    }
                }
                else if (route.Equals("/post") && request.Type == RequestType.POST)
                {
                    BodiesIn.CreatePost input = new BodiesIn.CreatePost(
                                                request.Data["title"],
                                                request.Data["description"],
                                                request.Data["path"]
                                            );
                    int userId = sessions[request.Data["sessionkey"]].userid;
                    answer = db.CreatePost(input, userId).ToJSON();
                }
                else if (route.Equals("/post/upload") && request.Type == RequestType.PUT)
                {
                    if (request.File != null)
                    {

                        File.WriteAllText(IMAGE_PATH + SUB_PATH + request.FileName, request.File, Encoding.Default);
                        answer = new ComplexAnswer(true, "file received", 200, new Upload(SUB_PATH.Replace('\\', '/') + request.FileName)).ToJSON();
                    }
                    else
                    {
                        answer = new NormalAnswer(true, "invite recieved", 200).ToJSON();
                    }
                }
                else if (route.Equals("/post/new") && request.Type == RequestType.GET)
                {
                    if (request.Data.ContainsKey("sessionkey"))
                    {
                        var userId = sessions[request.Data["sessionkey"]].userid;
                        answer = db.GetPostAndVote(false, userId).ToJSON();
                    }
                    else
                    {
                        answer = db.GetPost(false).ToJSON();
                    }
                }
                else if (route.Equals("/post/hot") && request.Type == RequestType.GET)
                {
                    if (request.Data.ContainsKey("sessionkey"))
                    {
                        var userId = sessions[request.Data["sessionkey"]].userid;
                        answer = db.GetPostAndVote(true, userId).ToJSON();
                    }
                    else
                    {
                        answer = db.GetPost(true).ToJSON();
                    }
                }
                else if (route.Equals("/post/vote") && request.Type == RequestType.PUT)
                {
                    var userId = sessions[request.Data["sessionkey"]].userid;
                    var value = Convert.ToInt32(request.Data["value"]);
                    var postId = Convert.ToInt32(request.Data["postId"]);
                    answer = db.VotePost(new Vote(postId, value, userId)).ToJSON();
                }
                #endregion
                #region Comment
                else if (route.Equals("/comment") && request.Type == RequestType.POST)
                {
                    var userId = sessions[request.Data["sessionkey"]].userid;
                    var text = request.Data["text"];
                    var postId = Convert.ToInt32(request.Data["postId"]);
                    var commentId = Convert.ToInt32(request.Data["commentId"]);
                    answer = db.CreateComment(new BodiesIn.Comment(text, postId, commentId), userId).ToJSON();
                }
                else if (route.Equals("/comment/vote") && request.Type == RequestType.PUT)
                {
                    var userId = sessions[request.Data["sessionkey"]].userid;
                    var value = Convert.ToInt32(request.Data["value"]);
                    var commentId = Convert.ToInt32(request.Data["commentId"]);
                    answer = db.VoteComment(new Vote(commentId, value, userId)).ToJSON();
                }
                #endregion
                else
                {
                    answer = new NormalAnswer(false, "internal server error [Unknown Path]", 500).ToJSON();
                }
            }
            catch (KeyNotFoundException)
            {
                answer = new NormalAnswer(false, $"internal server error [missing data]", 500).ToJSON();
            }
            catch (Exception e)
            {
                answer = new NormalAnswer(false, $"internal server error [{e.Message}]", 500).ToJSON();
            }
            #endregion
            return answer;
        }

        private string CreateSessionKey()
        {
            List<string> characters = new List<string>() {
                "a", "A", "b", "B", "c", "C", "d", "D", "e", "E",
                "f", "F", "g", "G", "h", "H", "i", "I", "j", "J",
                "k", "K", "l", "L", "m", "M", "n", "N", "o", "O",
                "p", "P", "q", "Q", "r", "R", "s", "S", "t", "T",
                "u", "U", "v", "V", "w", "W", "x", "X", "y", "Y",
                "z", "Z",
                "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"
            };

            StringBuilder sb = new StringBuilder("");
            Random rnd = new Random();

            for (int i = 0; i < 64; i++)
                sb.Append(characters[rnd.Next(0, characters.Count)]);

            return sb.ToString();
        }

    }
}
