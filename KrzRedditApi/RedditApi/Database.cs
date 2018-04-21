using RedditApi.BodiesIn;
using RedditApi.BodiesOut;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace RedditApi
{
    public class Database
    {
        private SQLiteConnection connection;

        public Database(string filename)
        {
            this.InitDatabase(filename);
        }

        #region Session
        public NormalAnswer Login(BodiesIn.Login input)
        {
            var command = new SQLiteCommand(this.connection);

            command.CommandText = $"SELECT userId FROM user WHERE username = '{input.username}' AND password = '{input.password}'";

            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                return new ComplexAnswer(true, "successful", 200, new LoginData("", Convert.ToInt32(reader[0].ToString())));
            }
            return new NormalAnswer(false, "wrong username or password", 400);
        }

        public NormalAnswer Registrieren(BodiesIn.Registration input)
        {
            var command = new SQLiteCommand(this.connection);

            command.CommandText = "SELECT username FROM user";
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                if ((string)reader[0] == input.username)
                {
                    return new NormalAnswer(false, "username already exists", 400);
                }
            }
            reader.Close();
            reader.Dispose();


            command.CommandText = $"INSERT INTO user (username, password) VALUES ('{input.username}', '{input.password}')";

            if (command.ExecuteNonQuery() > 0)
            {
                return new NormalAnswer(true, "successful", 200);
            }
            return new NormalAnswer(false, "internal server error [sql]", 500);
        }
        #endregion

        #region Post
        public NormalAnswer CreatePost(BodiesIn.CreatePost input, int userId)
        {
            var command = new SQLiteCommand(this.connection);

            command.CommandText = $"INSERT INTO post (title, beschreibung, pfad, userId) VALUES ('{input.title}', '{input.description}', '{input.path}', {userId})";

            if (command.ExecuteNonQuery() > 0)
            {
                return new NormalAnswer(true, "successful", 200);
            }
            return new NormalAnswer(false, "internal server error [sql]", 500);
        }

        public Post ParsePost(SQLiteDataReader reader)
        {
            int postId = Convert.ToInt32(reader[0].ToString());
            string title = reader[1].ToString();
            string desc = reader[2].ToString();
            string path = reader[3].ToString();
            string username = reader[4].ToString();
            int upvotes = Convert.ToInt32(reader[5].ToString());
            int downvotes = Convert.ToInt32(reader[6].ToString());
            return new Post(postId, title, desc, path, username, upvotes, downvotes);
        }
        public NormalAnswer GetPost(bool hot)
        {
            var command = new SQLiteCommand(this.connection);
            if (!hot)
            {
                command.CommandText = "SELECT p.userId, p.title, p.beschreibung, p.pfad, u.username, " +
                                    "(SELECT count(*) FROM vote_post v WHERE v.postId = p.postId AND v.value = 1) as upvotes, " +
                                    "(SELECT count(*) FROM vote_post v WHERE v.postId = p.postId AND v.value = -1) as downvotes " +
                                    "FROM post p, user u " +
                                    "WHERE p.userId = u.userId " +
                                    "ORDER BY p.postId DESC;";
            }
            else
            {
                command.CommandText = "SELECT p.userId, p.title, p.beschreibung, p.pfad, u.username, " +
                                    "(SELECT count(*) FROM vote_post v WHERE v.postId = p.postId AND v.value = 1) as upvotes, " +
                                    "(SELECT count(*) FROM vote_post v WHERE v.postId = p.postId AND v.value = -1) as downvotes, " +
                                    "IFNULL((SELECT sum(v.value) FROM vote_post v WHERE v.postId = p.postId), 0) as votes " +
                                    "FROM post p, user u " +
                                    "WHERE p.userId = u.userId " +
                                    "ORDER BY p.postId DESC;";
            }

            List<DataAnswer> output = new List<DataAnswer>();
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                output.Add(ParsePost(reader));
            }
            return new ComplexListAnswer(true, "successful", 200, output);
        }

        public Post ParsePostWithVote(SQLiteDataReader reader)
        {
            int postId = Convert.ToInt32(reader[0].ToString());
            string title = reader[1].ToString();
            string desc = reader[2].ToString();
            string path = reader[3].ToString();
            string username = reader[4].ToString();
            int upvotes = Convert.ToInt32(reader[5].ToString());
            int downvotes = Convert.ToInt32(reader[6].ToString());
            int yourvote = Convert.ToInt32(reader[7].ToString());
            return new Post(postId, title, desc, path, username, upvotes, downvotes, yourvote);
        }
        public NormalAnswer GetPostAndVote(bool hot, int userId)
        {
            var command = new SQLiteCommand(this.connection);
            if (!hot)
            {
                command.CommandText = "SELECT p.postId, p.title, p.beschreibung, p.pfad, u.username, " +
                                    "(SELECT count(*) FROM vote_post v WHERE v.postId = p.postId AND v.value = 1) as upvotes, " +
                                    "(SELECT count(*) FROM vote_post v WHERE v.postId = p.postId AND v.value = -1) as downvotes, " +
                                   $"IFNULL((SELECT v.value FROM vote_post v WHERE v.postId = p.postId AND v.userId = {userId}), 0) as yourvote " +
                                    "FROM post p, user u " +
                                    "WHERE p.userId = u.userId " +
                                    "ORDER BY p.postId DESC;";
            }
            else
            {
                command.CommandText = "SELECT p.postId, p.title, p.beschreibung, p.pfad, u.username, " +
                                    "(SELECT count(*) FROM vote_post v WHERE v.postId = p.postId AND v.value = 1) as upvotes, " +
                                    "(SELECT count(*) FROM vote_post v WHERE v.postId = p.postId AND v.value = -1) as downvotes, " +
                                   $"IFNULL((SELECT v.value FROM vote_post v WHERE v.postId = p.postId AND v.userId = {userId}), 0) as yourvote, " +
                                    "IFNULL((SELECT sum(v.value) FROM vote_post v WHERE v.postId = p.postId), 0) as votes " +
                                    "FROM post p, user u " +
                                    "WHERE p.userId = u.userId " +
                                    "ORDER BY p.postId DESC;";
            }

            List<DataAnswer> output = new List<DataAnswer>();
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                output.Add(ParsePostWithVote(reader));
            }
            return new ComplexListAnswer(true, "successful", 200, output);
        }

        public NormalAnswer GetSinglePost(int postId)
        {
            var command = new SQLiteCommand(this.connection);
            command.CommandText = $"SELECT p.postId, p.title, p.beschreibung, p.pfad, " +
                                    "(SELECT u.username FROM user u WHERE u.userId = p.userId) as username, " +
                                    "(SELECT count(*) FROM vote_post v WHERE v.postId = p.postId AND value = 1) as upvotes, " +
                                    "(SELECT count(*) FROM vote_post v WHERE v.postId = p.postId AND value = -1) as downvotes " +
                                    "FROM post p " +
                                    $"WHERE p.postId = {postId};";
            Post output = null;
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                output = ParsePost(reader);
            }
            if (output != null)
            {
                output.comment.AddRange(GetComments(output.postId, true));
                return new ComplexAnswer(true, "successful", 200, output);
            }
            return new NormalAnswer(false, "not found", 404);
        }
        public NormalAnswer GetSinglePost(int postId, int userId)
        {
            var command = new SQLiteCommand(this.connection);
            command.CommandText = $"SELECT p.postId, p.title, p.beschreibung, p.pfad, " +
                                    "(SELECT u.username FROM user u WHERE u.userId = p.userId) as username, " +
                                    "(SELECT count(*) FROM vote_post v WHERE v.postId = p.postId AND value = 1) as upvotes, " +
                                    "(SELECT count(*) FROM vote_post v WHERE v.postId = p.postId AND value = -1) as downvotes " +
                                    "FROM post p " +
                                    $"WHERE p.postId = {postId};";
            Post output = null;
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                output = ParsePost(reader);
            }
            if (output != null)
            {
                output.comment.AddRange(GetComments(output.postId, true, userId));
                return new ComplexAnswer(true, "successful", 200, output);
            }
            return new NormalAnswer(false, "not found", 404);
        }

        public NormalAnswer VotePost(Vote votePost)
        {
            bool update = false;

            var command = new SQLiteCommand(this.connection);

            command.CommandText = $"SELECT v.postId FROM vote_post v WHERE v.userId = {votePost.userId} " +
                                    $"AND v.postId = {votePost.id};";
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                if (Convert.ToInt32(reader[0].ToString()) == votePost.id)
                {
                    update = true;
                }
            }
            reader.Close();
            reader.Dispose();

            if (update)
            {
                command.CommandText = $"UPDATE vote_post SET " +
                    $"value = {votePost.value} " +
                    $"WHERE postId = {votePost.id} AND userId = { votePost.userId};";
            }
            else
            {
                command.CommandText = $"INSERT INTO vote_post (postId, userId, value) VALUES (" +
                    $"{votePost.id}, {votePost.userId}, {votePost.value});";
            }
            if (command.ExecuteNonQuery() > 0)
            {
                return new NormalAnswer(true, "successful", 200);
            }
            return new NormalAnswer(false, "internal server error [sql]", 500);
        }

        public NormalAnswer GetUser(int userId)
        {
            var command = new SQLiteCommand(this.connection);
            command.CommandText = $"SELECT u.userId, u.username FROM user u WHERE u.userId = {userId};";

            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                return new ComplexAnswer(true, "successful", 200, new UserData(userId, reader[1].ToString()));
            }
            return new NormalAnswer(false, "not found", 404);
        }
        #endregion

        #region Comment
        public NormalAnswer CreateComment(BodiesIn.Comment input, int userId)
        {
            var post = input.postId != -1;

            var command = new SQLiteCommand(this.connection);

            command.CommandText = "INSERT INTO comment (userId, superPostId, superCommentId, text) " +
                                 $"VALUES ({userId}, {input.postId}, {input.commentId}, '{input.text}');";

            if (command.ExecuteNonQuery() > 0)
            {
                command.CommandText = "select last_insert_rowid()";
                int id = (int)((Int64)command.ExecuteScalar());

                var comment = GetComment(id);

                if (comment != null)
                {
                    return new ComplexAnswer(true, "successful", 200, comment);
                }
            }
            return new NormalAnswer(false, "internal server error [sql]", 500);
        }

        public BodiesOut.Comment GetComment(int id)
        {
            var command = new SQLiteCommand(this.connection);
            command.CommandText = "SELECT commentId, text, " +
                                        "(SELECT username FROM user u WHERE u.userId = c.userId) as user, " +
                                        "(SELECT count(*) FROM vote_comment v WHERE v.commentId = c.commentId AND v.value = 1) as upvotes, " +
                                        "(SELECT count(*) FROM vote_comment v WHERE v.commentId = c.commentId AND v.value = -1) as downvotes " +
                                        "FROM comment c " +
                                       $"WHERE commentId = {id};";

            List<DataAnswer> output = new List<DataAnswer>();
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int commentId = Convert.ToInt32(reader[0].ToString());
                string text = reader[1].ToString();
                string user = reader[2].ToString();
                int upvotes = Convert.ToInt32(reader[3].ToString());
                int downvotes = Convert.ToInt32(reader[4].ToString());
                return new BodiesOut.Comment(commentId, text, user, upvotes, downvotes);
            }

            return null;
        }
        public BodiesOut.Comment GetComment(int id, int userId)
        {
            var command = new SQLiteCommand(this.connection);
            command.CommandText = "SELECT commentId, text, " +
                                        "(SELECT username FROM user u WHERE u.userId = c.userId) as user, " +
                                        "(SELECT count(*) FROM vote_comment v WHERE v.commentId = c.commentId AND v.value = 1) as upvotes, " +
                                        "(SELECT count(*) FROM vote_comment v WHERE v.commentId = c.commentId AND v.value = -1) as downvotes, " +
                                       $"IFNULL((SELECT v.value FROM vote_comment v WHERE v.commentId = c.commentId AND v.userId = {userId}), 0) as yourvote " +
                                        "FROM comment c " +
                                       $"WHERE commentId = {id};";

            List<DataAnswer> output = new List<DataAnswer>();
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int commentId = Convert.ToInt32(reader[0].ToString());
                string text = reader[1].ToString();
                string user = reader[2].ToString();
                int upvotes = Convert.ToInt32(reader[3].ToString());
                int downvotes = Convert.ToInt32(reader[4].ToString());
                int yourvote = Convert.ToInt32(reader[5].ToString());
                return new BodiesOut.Comment(commentId, text, user, upvotes, downvotes, yourvote);
            }

            return null;
        }

        public List<BodiesOut.Comment> GetComments(int id, bool post)
        {
            List<BodiesOut.Comment> comments = new List<BodiesOut.Comment>();
            var command = new SQLiteCommand(this.connection);
            command.CommandText = "SELECT commentId, text, " +
                                    "(SELECT username FROM user u WHERE u.userId = c.userId) as user, " +
                                    "(SELECT count(*) FROM vote_comment v WHERE v.commentId = c.commentId AND v.value = 1) as upvotes, " +
                                    "(SELECT count(*) FROM vote_comment v WHERE v.commentId = c.commentId AND v.value = -1) as downvotes, " +
                                    "IFNULL((SELECT sum(v.value) FROM vote_comment v WHERE v.commentId = c.commentId), 0) as votes " +
                                    "FROM comment c ";
            if (post)
                command.CommandText += $"WHERE superPostId = {id} ";
            else
                command.CommandText += $"WHERE superCommentId = {id} ";

            command.CommandText += "ORDER BY votes DESC;";

            List<DataAnswer> output = new List<DataAnswer>();
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int commentId = Convert.ToInt32(reader[0].ToString());
                string text = reader[1].ToString();
                string user = reader[2].ToString();
                int upvotes = Convert.ToInt32(reader[3].ToString());
                int downvotes = Convert.ToInt32(reader[4].ToString());
                var comment = new BodiesOut.Comment(commentId, text, user, upvotes, downvotes);
                comments.Add(comment);
            }

            foreach (var comment in comments)
            {
                comment.comment.AddRange(GetComments(comment.id, false));
            }

            return comments;
        }
        public List<BodiesOut.Comment> GetComments(int id, bool post, int userId)
        {
            List<BodiesOut.Comment> comments = new List<BodiesOut.Comment>();
            var command = new SQLiteCommand(this.connection);
            command.CommandText = "SELECT commentId, text, " +
                                    "(SELECT username FROM user u WHERE u.userId = c.userId) as user, " +
                                    "(SELECT count(*) FROM vote_comment v WHERE v.commentId = c.commentId AND v.value = 1) as upvotes, " +
                                    "(SELECT count(*) FROM vote_comment v WHERE v.commentId = c.commentId AND v.value = -1) as downvotes, " +
                                   $"IFNULL((SELECT v.value FROM vote_comment v WHERE v.commentId = c.commentId AND v.userId = {userId}), 0) as yourvote, " +
                                    "IFNULL((SELECT sum(v.value) FROM vote_comment v WHERE v.commentId = c.commentId), 0) as votes " +
                                    "FROM comment c ";
            if (post)
                command.CommandText += $"WHERE superPostId = {id} ";
            else
                command.CommandText += $"WHERE superCommentId = {id} ";

            command.CommandText += "ORDER BY votes DESC;";

            List<DataAnswer> output = new List<DataAnswer>();
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int commentId = Convert.ToInt32(reader[0].ToString());
                string text = reader[1].ToString();
                string user = reader[2].ToString();
                int upvotes = Convert.ToInt32(reader[3].ToString());
                int downvotes = Convert.ToInt32(reader[4].ToString());
                int yourvote = Convert.ToInt32(reader[5].ToString());
                var comment = new BodiesOut.Comment(commentId, text, user, upvotes, downvotes, yourvote);
                comments.Add(comment);
            }

            foreach (var comment in comments)
            {
                comment.comment.AddRange(GetComments(comment.id, false, userId));
            }

            return comments;
        }

        public NormalAnswer VoteComment(Vote vote)
        {
            bool update = false;

            var command = new SQLiteCommand(this.connection);

            command.CommandText = $"SELECT v.commentId FROM vote_comment v WHERE v.userId = {vote.userId} " +
                                    $"AND v.commentId = {vote.id};";
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                if (Convert.ToInt32(reader[0].ToString()) == vote.id)
                {
                    update = true;
                }
            }
            reader.Close();
            reader.Dispose();

            if (update)
            {
                command.CommandText = $"UPDATE vote_comment SET " +
                    $"value = {vote.value} " +
                    $"WHERE commentId = {vote.id} AND userId = { vote.userId};";
            }
            else
            {
                command.CommandText = $"INSERT INTO vote_comment (commentId, userId, value) VALUES (" +
                    $"{vote.id}, {vote.userId}, {vote.value});";
            }
            if (command.ExecuteNonQuery() > 0)
            {
                return new ComplexAnswer(true, "successful", 200, GetComment(vote.id, vote.userId));
            }
            return new NormalAnswer(false, "internal server error [sql]", 500);
        }
        #endregion

        private void InitDatabase(string filename)
        {
            Console.WriteLine("InitDatabase");

            if (System.IO.File.Exists(filename))
            {
                this.connection = new SQLiteConnection("Data Source=" + filename);
                this.connection.Open();
                Console.WriteLine("Use Existing Database: " + filename);
                return;
            }

            Console.WriteLine("Create Database: " + filename);
            if (System.IO.File.Exists(filename))
                System.IO.File.Move(filename, $"{filename}{DateTime.Now.Ticks}");
            SQLiteConnection.CreateFile(filename);
            this.connection = new SQLiteConnection("Data Source=" + filename);
            this.connection.Open();

            SQLiteCommand command;

            command = new SQLiteCommand(this.connection);
            CreateCommands(command);
            Console.WriteLine("Created: " + filename);
        }

        private void CreateCommands(SQLiteCommand command)
        {
            command.CommandText = "CREATE TABLE user (" +
                "userId INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                "username VARCHAR(64) NOT NULL UNIQUE, " +
                "password VARCHAR(512) NOT NULL);";
            command.ExecuteNonQuery();

            command.CommandText = "CREATE TABLE post (" +
                "postId INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                "title VARCHAR(64) NOT NULL, " +
                "beschreibung VARCHAR(512), " +
                "pfad VARCHAR(1024), " +
                "userId INTEGER NOT NULL);";
            command.ExecuteNonQuery();

            command.CommandText = "CREATE TABLE vote_post (" +
                "postId INTEGER NOT NULL, " +
                "userId INTEGER NOT NULL, " +
                "value INTEGER NOT NULL, " +
                "PRIMARY KEY (userId, postId));";
            command.ExecuteNonQuery();

            command.CommandText = "CREATE TABLE comment (" +
                "commentId INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                "userId INTEGER NOT NULL, " +
                "superPostId INTEGER NOT NULL, " +
                "superCommentId INTEGER NOT NULL, " +
                "text VARCHAR(512) NOT NULL);";
            command.ExecuteNonQuery();

            command.CommandText = "CREATE TABLE vote_comment (" +
                "commentId INTEGER NOT NULL, " +
                "userId INTEGER NOT NULL, " +
                "value INTEGER NOT NULL, " +
                "PRIMARY KEY (commentId, userId));";
            command.ExecuteNonQuery();
        }

    }
}
