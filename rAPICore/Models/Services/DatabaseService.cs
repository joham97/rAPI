using rAPI.Answers;
using rAPI.DTO;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace rAPI.Services
{
    public class DatabaseService
    {
        // Singleton
        public static readonly DatabaseService Instance = new DatabaseService(@"reddit.db");
        //public static readonly DatabaseService Instance = new DatabaseService(@"C:\Users\Administrator\Desktop\API\reddit.db");

        private SQLiteConnection connection;

        private DatabaseService(string filename)
        {
            this.InitDatabase(filename);
        }

        #region Session
        public NormalAnswer Login(Login input)
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

        public NormalAnswer Registrieren(Registration input)
        {
            var command = new SQLiteCommand(this.connection);

            command.CommandText = "SELECT username FROM user";
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                if ((string)reader[0] == input.username)
                {
                    return new NormalAnswer(false, "username already exists", 409);
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
        public NormalAnswer CreatePost(CreatePost input, int userId)
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
            int userId = Convert.ToInt32(reader[4].ToString());
            string username = reader[5].ToString();
            int upvotes = Convert.ToInt32(reader[6].ToString());
            int downvotes = Convert.ToInt32(reader[7].ToString());
            return new Post(postId, title, desc, path, userId, username, upvotes, downvotes);
        }
        public NormalAnswer GetPost(bool hot)
        {
            var command = new SQLiteCommand(this.connection);
            if (!hot)
            {
                command.CommandText = "SELECT p.postId, p.title, p.beschreibung, p.pfad, p.userId, u.username, " +
                                    "(SELECT count(*) FROM vote_post v WHERE v.postId = p.postId AND v.value = 1) as upvotes, " +
                                    "(SELECT count(*) FROM vote_post v WHERE v.postId = p.postId AND v.value = -1) as downvotes " +
                                    "FROM post p, user u " +
                                    "WHERE p.userId = u.userId " +
                                    "ORDER BY p.postId DESC;";
            }
            else
            {
                command.CommandText = "SELECT p.postId, p.title, p.beschreibung, p.pfad, p.userId, u.username, " +
                                    "(SELECT count(*) FROM vote_post v WHERE v.postId = p.postId AND v.value = 1) as upvotes, " +
                                    "(SELECT count(*) FROM vote_post v WHERE v.postId = p.postId AND v.value = -1) as downvotes, " +
                                    "IFNULL((SELECT sum(v.value) FROM vote_post v WHERE v.postId = p.postId), 0) as votes " +
                                    "FROM post p, user u " +
                                    "WHERE p.userId = u.userId " +
                                    "ORDER BY votes DESC;";
            }

            List<DataAnswer> output = new List<DataAnswer>();
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                output.Add(ParsePost(reader));
            }
            return new ComplexListAnswer(true, "successful", 200, output);
        }
        public NormalAnswer GetPostOfUser(int userId)
        {
            var command = new SQLiteCommand(this.connection);
            command.CommandText = "SELECT p.postId, p.title, p.beschreibung, p.pfad, p.userId, u.username, " +
                                    "(SELECT count(*) FROM vote_post v WHERE v.postId = p.postId AND v.value = 1) as upvotes, " +
                                    "(SELECT count(*) FROM vote_post v WHERE v.postId = p.postId AND v.value = -1) as downvotes " +
                                    "FROM post p, user u " +
                                   $"WHERE p.userId = {userId} AND p.userId = u.userId " +
                                    "ORDER BY p.postId DESC;";

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
            int userId = Convert.ToInt32(reader[4].ToString());
            string username = reader[5].ToString();
            int upvotes = Convert.ToInt32(reader[6].ToString());
            int downvotes = Convert.ToInt32(reader[7].ToString());
            int yourvote = Convert.ToInt32(reader[8].ToString());
            return new Post(postId, title, desc, path, userId, username, upvotes, downvotes, yourvote);
        }
        public NormalAnswer GetPostAndVote(bool hot, int userId)
        {
            var command = new SQLiteCommand(this.connection);
            if (!hot)
            {
                command.CommandText = "SELECT p.postId, p.title, p.beschreibung, p.pfad, p.userId, u.username, " +
                                    "(SELECT count(*) FROM vote_post v WHERE v.postId = p.postId AND v.value = 1) as upvotes, " +
                                    "(SELECT count(*) FROM vote_post v WHERE v.postId = p.postId AND v.value = -1) as downvotes, " +
                                   $"IFNULL((SELECT v.value FROM vote_post v WHERE v.postId = p.postId AND v.userId = {userId}), 0) as yourvote " +
                                    "FROM post p, user u " +
                                    "WHERE p.userId = u.userId " +
                                    "ORDER BY p.postId DESC;";
            }
            else
            {
                command.CommandText = "SELECT p.postId, p.title, p.beschreibung, p.pfad, p.userId, u.username, " +
                                    "(SELECT count(*) FROM vote_post v WHERE v.postId = p.postId AND v.value = 1) as upvotes, " +
                                    "(SELECT count(*) FROM vote_post v WHERE v.postId = p.postId AND v.value = -1) as downvotes, " +
                                   $"IFNULL((SELECT v.value FROM vote_post v WHERE v.postId = p.postId AND v.userId = {userId}), 0) as yourvote, " +
                                    "IFNULL((SELECT sum(v.value) FROM vote_post v WHERE v.postId = p.postId), 0) as votes " +
                                    "FROM post p, user u " +
                                    "WHERE p.userId = u.userId " +
                                    "ORDER BY votes DESC;";
            }

            List<DataAnswer> output = new List<DataAnswer>();
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                output.Add(ParsePostWithVote(reader));
            }
            return new ComplexListAnswer(true, "successful", 200, output);
        }
        public NormalAnswer GetPostAndVoteOfUser(int userId, int myUserId)
        {
            var command = new SQLiteCommand(this.connection);
            command.CommandText = "SELECT p.postId, p.title, p.beschreibung, p.pfad, p.userId, u.username, " +
                                    "(SELECT count(*) FROM vote_post v WHERE v.postId = p.postId AND v.value = 1) as upvotes, " +
                                    "(SELECT count(*) FROM vote_post v WHERE v.postId = p.postId AND v.value = -1) as downvotes, " +
                                   $"IFNULL((SELECT v.value FROM vote_post v WHERE v.postId = p.postId AND v.userId = {myUserId}), 0) as yourvote " +
                                    "FROM post p, user u " +
                                   $"WHERE p.userId = {userId} " +
                                    "ORDER BY p.postId DESC;";
            

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
            command.CommandText = $"SELECT p.postId, p.title, p.beschreibung, p.pfad, p.userId, " +
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
            command.CommandText = $"SELECT p.postId, p.title, p.beschreibung, p.pfad, p.userId, " +
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
            if(!HasPost(votePost.postId))
                return new NormalAnswer(false, "Post not found", 404);

            bool update = false;

            var command = new SQLiteCommand(this.connection);

            command.CommandText = $"SELECT v.postId FROM vote_post v WHERE v.userId = {votePost.userId} " +
                                    $"AND v.postId = {votePost.postId};";
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                if (Convert.ToInt32(reader[0].ToString()) == votePost.postId)
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
                    $"WHERE postId = {votePost.postId} AND userId = { votePost.userId};";
            }
            else
            {
                command.CommandText = $"INSERT INTO vote_post (postId, userId, value) VALUES (" +
                    $"{votePost.postId}, {votePost.userId}, {votePost.value});";
            }
            if (command.ExecuteNonQuery() > 0)
            {
                return new NormalAnswer(true, "successful", 200);
            }
            return new NormalAnswer(false, "internal server error [sql]", 500);
        }

        public NormalAnswer DeletePost(int postId)
        {
            var command = new SQLiteCommand(this.connection);
            command.CommandText = $"DELETE FROM post WHERE postId = {postId};";
            command.ExecuteNonQuery();

            List<long> subComments = new List<long>();

            command.CommandText = $"SELECT commentId FROM comment WHERE superPostId = {postId};";
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                subComments.Add((long)reader["commentId"]);
            }

            foreach (int subCommentId in subComments)
            {
                this.DeleteComment(subCommentId);
            }

            return new NormalAnswer(true, "successful", 200);
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

        public bool IsUserOwnerOfPost(int postId, int userId)
        {
            var command = new SQLiteCommand(this.connection);
            command.CommandText = $"SELECT COUNT(*) as count FROM Post WHERE postId = {postId} AND userId = {userId};";
            command.ExecuteNonQuery();

            SQLiteDataReader reader = command.ExecuteReader();
            reader.Read();

            return (long)reader["count"] == 1;
        }

        public bool HasPost(int postId)
        {
            var command = new SQLiteCommand(this.connection);
            command.CommandText = $"SELECT COUNT(*) as count FROM Post WHERE postId = {postId};";
            command.ExecuteNonQuery();

            SQLiteDataReader reader = command.ExecuteReader();
            reader.Read();

            return (long)reader["count"] == 1;
        }
        #endregion

        #region Comment
        public NormalAnswer CreateComment(CreateComment input, int userId)
        {
            if (input.postId >= 0 && !GetSinglePost(input.postId).success)
                return new NormalAnswer(false, "Post not found", 404);

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

        public Comment GetComment(int id)
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
                return new Comment(commentId, text, user, upvotes, downvotes);
            }

            return null;
        }
        public Comment GetComment(int id, int userId)
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
                return new Comment(commentId, text, user, upvotes, downvotes, yourvote);
            }

            return null;
        }

        public List<Comment> GetComments(int id, bool post)
        {
            List<Comment> comments = new List<Comment>();
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
                var comment = new Comment(commentId, text, user, upvotes, downvotes);
                comments.Add(comment);
            }

            foreach (var comment in comments)
            {
                comment.comment.AddRange(GetComments(comment.commentId, false));
            }

            return comments;
        }
        public List<Comment> GetComments(int id, bool post, int userId)
        {
            List<Comment> comments = new List<Comment>();
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
                var comment = new Comment(commentId, text, user, upvotes, downvotes, yourvote);
                comments.Add(comment);
            }

            foreach (var comment in comments)
            {
                comment.comment.AddRange(GetComments(comment.commentId, false, userId));
            }

            return comments;
        }

        public NormalAnswer VoteComment(CommentVote vote, int userId)
        {
            if(!HasComment(vote.commentId))
                return new NormalAnswer(false, "Comment not found", 404);

            bool update = false;

            var command = new SQLiteCommand(this.connection);

            command.CommandText = $"SELECT v.commentId FROM vote_comment v WHERE v.userId = {userId} " +
                                    $"AND v.commentId = {vote.commentId};";
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                if (Convert.ToInt32(reader[0].ToString()) == vote.commentId)
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
                    $"WHERE commentId = {vote.commentId} AND userId = {userId};";
            }
            else
            {
                command.CommandText = $"INSERT INTO vote_comment (commentId, userId, value) VALUES (" +
                    $"{vote.commentId}, {userId}, {vote.value});";
            }
            if (command.ExecuteNonQuery() > 0)
            {
                return new ComplexAnswer(true, "successful", 200, GetComment(vote.commentId, userId));
            }
            return new NormalAnswer(false, "internal server error [sql]", 500);
        }

        public NormalAnswer DeleteComment(int commentId)
        {
            if (!HasComment(commentId))
                return new NormalAnswer(false, "Comment not found", 404);

            var command = new SQLiteCommand(this.connection);

            command.CommandText = $"DELETE FROM comment WHERE commentId = {commentId};";
            command.ExecuteNonQuery();

            List<long> subComments = new List<long>();

            command.CommandText = $"SELECT commentId FROM comment WHERE superCommentId = {commentId};";
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                subComments.Add((long)reader["commentId"]);
            }

            foreach (int subCommentId in subComments)
            {
                this.DeleteComment(subCommentId);
            }

            return new NormalAnswer(true, "successful", 200);
        }
        
        public bool HasComment(int commentId)
        {
            var command = new SQLiteCommand(this.connection);
            command.CommandText = $"SELECT COUNT(*) as count FROM comment WHERE commentId = {commentId};";
            command.ExecuteNonQuery();

            SQLiteDataReader reader = command.ExecuteReader();
            reader.Read();

            return (long)reader["count"] == 1;
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

        public void ClearDatabase()
        {
            var command = new SQLiteCommand(this.connection);
            command.CommandText = @"
                DELETE FROM vote_comment;
                DELETE FROM vote_post;
                DELETE FROM comment;
                DELETE FROM post;
                DELETE FROM user;
                DELETE FROM sqlite_sequence;
            ";
            command.ExecuteNonQuery();
        }
    }
}
