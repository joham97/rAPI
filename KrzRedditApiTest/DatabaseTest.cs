using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedditApi;

namespace RedditApiTest
{
    class DatabaseTest
    {

        public static void Test()
        {
            File.Delete("test.db");
            Database db = new Database("test.db");

            Console.WriteLine();
            
            NormalAnswer register = db.Registrieren(new RedditApi.BodiesIn.Registration("testUser", "testUser"));
            Console.WriteLine($"Register: {register.success}");

            NormalAnswer login = db.Login(new RedditApi.BodiesIn.Login("testUser", "testUser"));
            var userId = ((login as ComplexAnswer).data as RedditApi.BodiesOut.LoginData).userId;
            Console.WriteLine($"Login: {login.success}");

            NormalAnswer post1 = db.CreatePost(new RedditApi.BodiesIn.CreatePost("test 1", "this is test 1", "/assets/Test321.png"), userId);
            Console.WriteLine($"Create Post 1: {post1.success}");

            NormalAnswer post2 = db.CreatePost(new RedditApi.BodiesIn.CreatePost("test 2", "this is test 2", null), userId);
            Console.WriteLine($"Create Post 2: {post2.success}");

            NormalAnswer hotPosts = db.GetPost(true);
            Console.WriteLine($"Hot Posts: {hotPosts.success}");

            NormalAnswer newPosts = db.GetPost(false);
            Console.WriteLine($"New Posts: {newPosts.success}");

            NormalAnswer hotPostsVotes = db.GetPostAndVote(true, userId);
            Console.WriteLine($"Hot Posts /w Votes: {hotPostsVotes.success}");

            NormalAnswer newPostsVotes = db.GetPostAndVote(false, userId);
            Console.WriteLine($"New Posts /w Votes: {newPostsVotes.success}");

            NormalAnswer vote1 = db.VotePost(new RedditApi.BodiesIn.Vote(1, 1, userId));
            Console.WriteLine($"Vote 1: {vote1.success}");

            NormalAnswer vote2 = db.VotePost(new RedditApi.BodiesIn.Vote(2, -1, userId));
            Console.WriteLine($"Vote 2: {vote2.success}");

            NormalAnswer comment1 = db.CreateComment(new RedditApi.BodiesIn.Comment("Testkommentar", 1, -1), 1);
            Console.WriteLine($"Comment 1: {comment1.success}");

            NormalAnswer comment2 = db.CreateComment(new RedditApi.BodiesIn.Comment("Testsubkommentar", -1, 1), 1);
            Console.WriteLine($"Comment 2: {comment2.success}");

            NormalAnswer comment3 = db.CreateComment(new RedditApi.BodiesIn.Comment("2. Testkommentar", 1, -1), 1);
            Console.WriteLine($"Comment 3: {comment3.success}");

            NormalAnswer voteComment1 = db.VoteComment(new RedditApi.BodiesIn.Vote(1, 1, userId));
            Console.WriteLine($"Vote Comment 1: {voteComment1.success}");
            Console.WriteLine($"JSON: {voteComment1.ToJSON()}");

            NormalAnswer singlePost = db.GetSinglePost(1);
            Console.WriteLine($"Single Post: {singlePost.success}");
            //Console.WriteLine($"JSON: {singlePost.ToJSON()}");

            Console.ReadLine();
        }

    }
}
