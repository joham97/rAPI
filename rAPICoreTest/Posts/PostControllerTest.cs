using Microsoft.AspNetCore.Mvc;
using rAPI.Answers;
using rAPI.Controllers;
using rAPI.DTO;
using System.Collections.Generic;
using Xunit;

namespace rAPICoreTest
{
    public class PostControllerTest
    {
        private CreatePost[] createPosts = new CreatePost[] {
            new CreatePost("Testpost", "This is a testpost", "/image.png"),
            new CreatePost("Post le test", "This is a post le test", "/image2.png"),
            new CreatePost("Testing posting", "This is a testing posting", "/image.3png")
        };

        private Vote upvote = new Vote(1, 1, 1);
        private Vote downvote = new Vote(1, -1, 1);

        [Fact]
        public LoginData CreatePost()
        {
            // Arrange
            LoginData loginData = TestSetup.TestLogin();

            PostController postController = new PostController();

            // Act
            var actionResult = postController.Post(loginData.sessionkey, createPosts[0]).Result;
            var actual = (actionResult as OkObjectResult).Value as NormalAnswer;

            // Assert            
            Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(TestSetup.NormalAnswer.code, actual.code);
            Assert.Equal(TestSetup.NormalAnswer.message, actual.message);
            Assert.Equal(TestSetup.NormalAnswer.success, actual.success);

            return loginData;
        }

        [Fact]
        public void GetPost()
        {
            // Arrange
            LoginData loginData = CreatePost();

            PostController postController = new PostController();

            // Act
            var actionResult = postController.Get(1, loginData.sessionkey).Result;
            var actual = ((actionResult as OkObjectResult).Value as ComplexAnswer).data as Post;

            // Assert            
            Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(createPosts[0].title, actual.title);
            Assert.Equal(createPosts[0].description, actual.description);
            Assert.Equal(createPosts[0].path, actual.path);
        }

        [Fact]
        public void DeletePost()
        {
            // Arrange
            LoginData loginData = CreatePost();

            PostController postController = new PostController();

            var getPostResult = postController.Get(1, loginData.sessionkey).Result;
            var createdPost = ((getPostResult as OkObjectResult).Value as ComplexAnswer).data as Post;

            // Act
            postController.Delete(createdPost.postId, loginData.sessionkey);

            // Assert           
            var getPostNotFoundResult = postController.Get(1, loginData.sessionkey).Result;
            Assert.IsType<NotFoundResult>(getPostNotFoundResult);
        }

        [Fact]
        public void GetNewPosts()
        {
            // Arrange
            LoginData loginData = TestSetup.TestLogin();

            PostController postController = new PostController();

            // Act
            postController.Post(loginData.sessionkey, createPosts[0]);
            postController.Post(loginData.sessionkey, createPosts[1]);
            postController.Post(loginData.sessionkey, createPosts[2]);

            // Act
            PostsController postsController = new PostsController();
            var actionResult = postsController.Get("new", loginData.sessionkey).Result;
            var actual = ((actionResult as OkObjectResult).Value as ComplexListAnswer).data as List<DataAnswer>;

            // Assert            
            Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(createPosts.Length, actual.Count);
            for (int i = 0; i < 3; i++)
            {
                var singleActual = actual[2 - i] as Post;
                Assert.Equal(createPosts[i].title, singleActual.title);
                Assert.Equal(createPosts[i].description, singleActual.description);
                Assert.Equal(createPosts[i].path, singleActual.path);
            }
        }

        [Fact]
        public void GetHotPosts()
        {
            // Arrange
            LoginData loginData = TestSetup.TestLogin();

            PostController postController = new PostController();

            // Act
            postController.Post(loginData.sessionkey, createPosts[0]);
            postController.Post(loginData.sessionkey, createPosts[1]);
            postController.Post(loginData.sessionkey, createPosts[2]);

            // Act
            PostsController postsController = new PostsController();
            var actionResult = postsController.Get("hot", loginData.sessionkey).Result;
            var actual = ((actionResult as OkObjectResult).Value as ComplexListAnswer).data as List<DataAnswer>;

            // Assert            
            Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(createPosts.Length, actual.Count);
            for (int i = 0; i < 3; i++)
            {
                var singleActual = actual[i] as Post;
                Assert.Equal(createPosts[i].title, singleActual.title);
                Assert.Equal(createPosts[i].description, singleActual.description);
                Assert.Equal(createPosts[i].path, singleActual.path);
            }
        }

        [Fact]
        public void UpvotePost()
        {
            // Arrange
            LoginData loginData = CreatePost();

            PostController postController = new PostController();
            var getPostResult = postController.Get(1, loginData.sessionkey).Result;
            var createdPost = ((getPostResult as OkObjectResult).Value as ComplexAnswer).data as Post;

            VoteController voteController = new VoteController();

            // Act
            voteController.Post(loginData.sessionkey, upvote);

            // Assert
            var actionResult = postController.Get(1, loginData.sessionkey).Result;
            var actual = ((actionResult as OkObjectResult).Value as ComplexAnswer).data as Post;

            Assert.Equal(createPosts[0].title, actual.title);
            Assert.Equal(createPosts[0].description, actual.description);
            Assert.Equal(createPosts[0].path, actual.path);
            Assert.Equal(1, actual.upvotes);
        }

        [Fact]
        public void DownvotePost()
        {
            // Arrange
            LoginData loginData = CreatePost();

            PostController postController = new PostController();
            var getPostResult = postController.Get(1, loginData.sessionkey).Result;
            var createdPost = ((getPostResult as OkObjectResult).Value as ComplexAnswer).data as Post;

            VoteController voteController = new VoteController();

            // Act
            voteController.Post(loginData.sessionkey, downvote);

            // Assert
            var actionResult = postController.Get(1, loginData.sessionkey).Result;
            var actual = ((actionResult as OkObjectResult).Value as ComplexAnswer).data as Post;

            Assert.Equal(createPosts[0].title, actual.title);
            Assert.Equal(createPosts[0].description, actual.description);
            Assert.Equal(createPosts[0].path, actual.path);
            Assert.Equal(1, actual.downvotes);
        }

    }
}
