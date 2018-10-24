using rAPI.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace rAPICoreTest
{
    class Mocks
    {

        public static readonly CreatePost[] createPosts = new CreatePost[] {
            new CreatePost("Testpost", "This is a testpost", "/image.png"),
            new CreatePost("Post le test", "This is a post le test", "/image2.png"),
            new CreatePost("Testing posting", "This is a testing posting", "/image.3png")
        };

        public static readonly Vote upvote = new Vote(1, 1, 1);
        public static readonly Vote downvote = new Vote(1, -1, 1);

        public static readonly CreatePost post = new CreatePost("Testpost", "This is a testpost", "/image.png");

        public static readonly CreateComment comment1 = new CreateComment("Testkommentar", 1, -1);
        public static readonly CreateComment comment2 = new CreateComment("Testkommentar", 1, -1);
        public static readonly CreateComment subComment = new CreateComment("Testsubkommentar", -1, 1);

        public static readonly CommentVote voteOnCommentPositive = new CommentVote(1, 1);
        public static readonly CommentVote voteOnCommentNegative = new CommentVote(1, -1);
        public static readonly CommentVote voteOnCommentNegative2 = new CommentVote(2, -1);

    }
}
