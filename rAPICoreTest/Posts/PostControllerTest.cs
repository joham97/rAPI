using Microsoft.AspNetCore.Mvc;
using rAPI.Answers;
using rAPI.Controllers;
using rAPI.DTO;
using rAPI.Services;
using System.Collections.Generic;
using Xunit;

namespace rAPICoreTest
{
    public class PostControllerTest
    {
        
        [Fact]
        public LoginData CreatePost()
        {
            // Arrange
            LoginData loginData = TestSetup.TestLogin();

            PostController postController = new PostController();

            // Act
            var actionResult = postController.Post(loginData.sessionkey, Mocks.createPosts[0]).Result;
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
            var actionResult = postController.Get(loginData.sessionkey, 1).Result;
            var actual = ((actionResult as OkObjectResult).Value as ComplexAnswer).data as Post;

            // Assert            
            Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(Mocks.createPosts[0].title, actual.title);
            Assert.Equal(Mocks.createPosts[0].description, actual.description);
            Assert.Equal(Mocks.createPosts[0].path, actual.path);
            Assert.Equal(1, actual.userId);
            Assert.Equal("testusername", actual.user);
        }

        [Fact]
        public void DeletePost()
        {
            // Arrange
            LoginData loginData = CreatePost();

            PostController postController = new PostController();

            var getPostResult = postController.Get(loginData.sessionkey, 1).Result;
            var createdPost = ((getPostResult as OkObjectResult).Value as ComplexAnswer).data as Post;

            // Act
            postController.Delete(loginData.sessionkey, createdPost.postId);

            // Assert           
            var getPostNotFoundResult = postController.Get(loginData.sessionkey, 1).Result;
            Assert.IsType<NotFoundObjectResult>(getPostNotFoundResult);
        }

        [Fact]
        public void GetNewPosts()
        {
            // Arrange
            LoginData loginData = TestSetup.TestLogin();

            PostController postController = new PostController();

            // Act
            postController.Post(loginData.sessionkey, Mocks.createPosts[0]);
            postController.Post(loginData.sessionkey, Mocks.createPosts[1]);
            postController.Post(loginData.sessionkey, Mocks.createPosts[2]);

            // Act
            PostsController postsController = new PostsController();
            var actionResult = postsController.Get(loginData.sessionkey, "new", null).Result;
            var actual = ((actionResult as OkObjectResult).Value as ComplexListAnswer).data as List<DataAnswer>;

            // Assert            
            Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(Mocks.createPosts.Length, actual.Count);
            for (int i = 0; i < 3; i++)
            {
                var singleActual = actual[2 - i] as Post;
                Assert.Equal(Mocks.createPosts[i].title, singleActual.title);
                Assert.Equal(Mocks.createPosts[i].description, singleActual.description);
                Assert.Equal(Mocks.createPosts[i].path, singleActual.path);
            }
        }

        [Fact]
        public void GetNewPostsWithoutSessionkey()
        {
            // Arrange
            LoginData loginData = TestSetup.TestLogin();

            PostController postController = new PostController();

            // Act
            postController.Post(loginData.sessionkey, Mocks.createPosts[0]);
            postController.Post(loginData.sessionkey, Mocks.createPosts[1]);
            postController.Post(loginData.sessionkey, Mocks.createPosts[2]);

            // Act
            PostsController postsController = new PostsController();
            var actionResult = postsController.Get(null, "new", null).Result;
            var actual = ((actionResult as OkObjectResult).Value as ComplexListAnswer).data as List<DataAnswer>;

            // Assert            
            Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(Mocks.createPosts.Length, actual.Count);
            for (int i = 0; i < 3; i++)
            {
                var singleActual = actual[2 - i] as Post;
                Assert.Equal(Mocks.createPosts[i].title, singleActual.title);
                Assert.Equal(Mocks.createPosts[i].description, singleActual.description);
                Assert.Equal(Mocks.createPosts[i].path, singleActual.path);
            }
        }

        [Fact]
        public void GetHotPosts()
        {
            // Arrange
            LoginData loginData = TestSetup.TestLogin();

            PostController postController = new PostController();

            // Act
            postController.Post(loginData.sessionkey, Mocks.createPosts[0]);
            postController.Post(loginData.sessionkey, Mocks.createPosts[1]);
            postController.Post(loginData.sessionkey, Mocks.createPosts[2]);

            // Act
            PostsController postsController = new PostsController();
            var actionResult = postsController.Get(loginData.sessionkey, "hot", null).Result;
            var actual = ((actionResult as OkObjectResult).Value as ComplexListAnswer).data as List<DataAnswer>;

            // Assert            
            Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(Mocks.createPosts.Length, actual.Count);
            for (int i = 0; i < 3; i++)
            {
                var singleActual = actual[i] as Post;
                Assert.Equal(Mocks.createPosts[i].title, singleActual.title);
                Assert.Equal(Mocks.createPosts[i].description, singleActual.description);
                Assert.Equal(Mocks.createPosts[i].path, singleActual.path);
            }
        }

        [Fact]
        public void GetHotPostsWithoutSessionkey()
        {
            // Arrange
            LoginData loginData = TestSetup.TestLogin();

            PostController postController = new PostController();

            // Act
            postController.Post(loginData.sessionkey, Mocks.createPosts[0]);
            postController.Post(loginData.sessionkey, Mocks.createPosts[1]);
            postController.Post(loginData.sessionkey, Mocks.createPosts[2]);

            // Act
            PostsController postsController = new PostsController();
            var actionResult = postsController.Get(null, "hot", null).Result;
            var actual = ((actionResult as OkObjectResult).Value as ComplexListAnswer).data as List<DataAnswer>;

            // Assert            
            Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(Mocks.createPosts.Length, actual.Count);
            for (int i = 0; i < 3; i++)
            {
                var singleActual = actual[i] as Post;
                Assert.Equal(Mocks.createPosts[i].title, singleActual.title);
                Assert.Equal(Mocks.createPosts[i].description, singleActual.description);
                Assert.Equal(Mocks.createPosts[i].path, singleActual.path);
            }
        }

        [Fact]
        public void GetUserPosts()
        {
            // Arrange
            LoginData loginData = TestSetup.TestLogin();

            PostController postController = new PostController();

            // Act
            postController.Post(loginData.sessionkey, Mocks.createPosts[0]);
            postController.Post(loginData.sessionkey, Mocks.createPosts[1]);
            postController.Post(loginData.sessionkey, Mocks.createPosts[2]);

            // Act
            PostsController postsController = new PostsController();
            var actionResult = postsController.Get(loginData.sessionkey, "user", "1").Result;
            var actual = ((actionResult as OkObjectResult).Value as ComplexListAnswer).data as List<DataAnswer>;

            // Assert            
            Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(Mocks.createPosts.Length, actual.Count);
            for (int i = 0; i < 3; i++)
            {
                var singleActual = actual[2 - i] as Post;
                Assert.Equal(Mocks.createPosts[i].title, singleActual.title);
                Assert.Equal(Mocks.createPosts[i].description, singleActual.description);
                Assert.Equal(Mocks.createPosts[i].path, singleActual.path);
            }
        }
        
        [Fact]
        public void GetUserPostsWithoutSessionkey()
        {
            // Arrange
            LoginData loginData = TestSetup.TestLogin();

            PostController postController = new PostController();

            // Act
            postController.Post(loginData.sessionkey, Mocks.createPosts[0]);
            postController.Post(loginData.sessionkey, Mocks.createPosts[1]);
            postController.Post(loginData.sessionkey, Mocks.createPosts[2]);

            // Act
            PostsController postsController = new PostsController();
            var actionResult = postsController.Get(null, "user", "1").Result;
            var actual = ((actionResult as OkObjectResult).Value as ComplexListAnswer).data as List<DataAnswer>;

            // Assert            
            Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(Mocks.createPosts.Length, actual.Count);
            for (int i = 0; i < 3; i++)
            {
                var singleActual = actual[2 - i] as Post;
                Assert.Equal(Mocks.createPosts[i].title, singleActual.title);
                Assert.Equal(Mocks.createPosts[i].description, singleActual.description);
                Assert.Equal(Mocks.createPosts[i].path, singleActual.path);
            }
        }


        [Fact]
        public void UpvotePost()
        {
            // Arrange
            LoginData loginData = CreatePost();

            PostController postController = new PostController();
            var getPostResult = postController.Get(loginData.sessionkey, 1).Result;
            var createdPost = ((getPostResult as OkObjectResult).Value as ComplexAnswer).data as Post;

            VoteController voteController = new VoteController();

            // Act
            var actionResult = voteController.Post(loginData.sessionkey, Mocks.upvote).Result;

            // Assert
            var actual = ((actionResult as OkObjectResult).Value as ComplexAnswer).data as Post;

            Assert.Equal(Mocks.createPosts[0].title, actual.title);
            Assert.Equal(Mocks.createPosts[0].description, actual.description);
            Assert.Equal(Mocks.createPosts[0].path, actual.path);
            Assert.Equal(1, actual.upvotes);
            Assert.Equal(0, actual.downvotes);
            Assert.Equal(1, actual.yourvote);
        }

        [Fact]
        public void UpvoteAndDownvoteSamePost()
        {
            // Arrange
            LoginData loginData = CreatePost();

            PostController postController = new PostController();
            var getPostResult = postController.Get(loginData.sessionkey, 1).Result;
            var createdPost = ((getPostResult as OkObjectResult).Value as ComplexAnswer).data as Post;

            VoteController voteController = new VoteController();

            // Act
            voteController.Post(loginData.sessionkey, Mocks.downvote);
            voteController.Post(loginData.sessionkey, Mocks.upvote);

            // Assert
            var actionResult = postController.Get(loginData.sessionkey, 1).Result;
            var actual = ((actionResult as OkObjectResult).Value as ComplexAnswer).data as Post;

            Assert.Equal(Mocks.createPosts[0].title, actual.title);
            Assert.Equal(Mocks.createPosts[0].description, actual.description);
            Assert.Equal(Mocks.createPosts[0].path, actual.path);
            Assert.Equal(1, actual.upvotes);
        }

        [Fact]
        public void DownvotePost()
        {
            // Arrange
            LoginData loginData = CreatePost();

            PostController postController = new PostController();
            var getPostResult = postController.Get(loginData.sessionkey, 1).Result;
            var createdPost = ((getPostResult as OkObjectResult).Value as ComplexAnswer).data as Post;

            VoteController voteController = new VoteController();
            
            // Act
            var actionResult = voteController.Post(loginData.sessionkey, Mocks.downvote).Result;

            // Assert
            var actual = ((actionResult as OkObjectResult).Value as ComplexAnswer).data as Post;

            Assert.Equal(Mocks.createPosts[0].title, actual.title);
            Assert.Equal(Mocks.createPosts[0].description, actual.description);
            Assert.Equal(Mocks.createPosts[0].path, actual.path);
            Assert.Equal(0, actual.upvotes);
            Assert.Equal(1, actual.downvotes);
            Assert.Equal(-1, actual.yourvote);
        }


        [Fact]
        public LoginData GetAdvancedPost()
        {
            // Arrange
            LoginData loginData = CreatePost();

            PostController postController = new PostController();
            CommentController commentController = new CommentController();
            VoteCommentController voteCommentController = new VoteCommentController();

            commentController.Post(loginData.sessionkey, Mocks.comment1);
            commentController.Post(loginData.sessionkey, Mocks.comment2);
            commentController.Post(loginData.sessionkey, Mocks.subComment);
            voteCommentController.Post(loginData.sessionkey, Mocks.voteOnCommentPositive);
            voteCommentController.Post(loginData.sessionkey, Mocks.voteOnCommentNegative2);
            postController.Get(loginData.sessionkey, 1);

            // Act
            var actionResult = postController.Get(loginData.sessionkey, 1).Result;
            var actual = (actionResult as OkObjectResult).Value as ComplexAnswer;
            var data = actual.data as Post;

            // Assert            
            Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(TestSetup.NormalAnswer.code, actual.code);
            Assert.Equal(TestSetup.NormalAnswer.message, actual.message);
            Assert.Equal(TestSetup.NormalAnswer.success, actual.success);

            Assert.Equal(Mocks.post.title, data.title);
            Assert.Equal(Mocks.post.description, data.description);
            Assert.Equal(Mocks.post.path, data.path);

            Assert.Equal(Mocks.comment1.text, data.comment[0].text);
            Assert.Equal(1, data.comment[0].upvotes);
            Assert.Equal(0, data.comment[0].downvotes);
            Assert.Equal(1, data.comment[0].yourvote);

            Assert.Equal(Mocks.comment2.text, data.comment[1].text);
            Assert.Equal(0, data.comment[1].upvotes);
            Assert.Equal(1, data.comment[1].downvotes);
            Assert.Equal(-1, data.comment[1].yourvote);

            Assert.Equal(Mocks.subComment.text, data.comment[0].comment[0].text);
            Assert.Equal(0, data.comment[0].comment[0].downvotes);
            Assert.Equal(0, data.comment[0].comment[0].upvotes);
            Assert.Equal(0, data.comment[0].comment[0].yourvote);

            return loginData;
        }

        [Fact]
        public LoginData GetAdvancedPostWithoutSessionkey()
        {
            // Arrange
            LoginData loginData = CreatePost();

            PostController postController = new PostController();
            CommentController commentController = new CommentController();
            VoteCommentController voteCommentController = new VoteCommentController();

            commentController.Post(loginData.sessionkey, Mocks.comment1);
            commentController.Post(loginData.sessionkey, Mocks.comment2);
            commentController.Post(loginData.sessionkey, Mocks.subComment);
            voteCommentController.Post(loginData.sessionkey, Mocks.voteOnCommentPositive);
            voteCommentController.Post(loginData.sessionkey, Mocks.voteOnCommentNegative2);
            postController.Get(loginData.sessionkey, 1);

            // Act
            var actionResult = postController.Get(null, 1).Result;
            var actual = (actionResult as OkObjectResult).Value as ComplexAnswer;
            var data = actual.data as Post;

            // Assert            
            Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(TestSetup.NormalAnswer.code, actual.code);
            Assert.Equal(TestSetup.NormalAnswer.message, actual.message);
            Assert.Equal(TestSetup.NormalAnswer.success, actual.success);

            Assert.Equal(Mocks.post.title, data.title);
            Assert.Equal(Mocks.post.description, data.description);
            Assert.Equal(Mocks.post.path, data.path);

            Assert.Equal(Mocks.comment1.text, data.comment[0].text);
            Assert.Equal(1, data.comment[0].upvotes);
            Assert.Equal(0, data.comment[0].downvotes);
            Assert.Equal(0, data.comment[0].yourvote);

            Assert.Equal(Mocks.comment2.text, data.comment[1].text);
            Assert.Equal(0, data.comment[1].upvotes);
            Assert.Equal(1, data.comment[1].downvotes);
            Assert.Equal(0, data.comment[1].yourvote);

            Assert.Equal(Mocks.subComment.text, data.comment[0].comment[0].text);
            Assert.Equal(0, data.comment[0].comment[0].downvotes);
            Assert.Equal(0, data.comment[0].comment[0].upvotes);
            Assert.Equal(0, data.comment[0].comment[0].yourvote);

            return loginData;
        }
        [Fact]
        public void GetAdvancedPostFailed()
        {
            // Arrange
            Assert.IsType<UnauthorizedResult>(new PostController().Get("", 1).Result);
        }

        [Fact]
        public void DeleteWholePost()
        {
            // Arrange
            LoginData loginData = GetAdvancedPost();

            PostController postController = new PostController();

            // Act
            postController.Delete(loginData.sessionkey, 1);

            //Assert
            var comment = DatabaseService.Instance.GetComment(1);
            var subComment = DatabaseService.Instance.GetComment(3);

            Assert.Null(comment);
            Assert.Null(subComment);
        }

        #region Unauthorized
        [Fact]
        public void CreatePostUnauthorized()
        {
            //Arrange
            PostController postController = new PostController();

            // Act
            var actionResult = postController.Post("", Mocks.post).Result;

            // Assert
            Assert.IsType<UnauthorizedResult>(actionResult);
        }

        [Fact]
        public void DeletePostUnauthorized()
        {
            //Arrange
            PostController postController = new PostController();

            // Act
            var actionResult = postController.Delete("", 1).Result;

            // Assert
            Assert.IsType<UnauthorizedResult>(actionResult);
        }

        [Fact]
        public void VoteUnauthorized()
        {
            Assert.IsType<UnauthorizedResult>(new VoteController().Post("", Mocks.upvote).Result);

            LoginData loginData = TestSetup.TestLogin();
            Assert.IsType<NotFoundObjectResult>(new VoteController().Post(loginData.sessionkey, new Vote(123, 1, 1)).Result);
        }

        [Fact]
        public void GetPostsUnauthorized()
        {
            Assert.IsType<UnauthorizedResult>(new PostsController().Get("", "hot", null).Result);
            Assert.IsType<UnauthorizedResult>(new PostsController().Get("", "new", null).Result);
            Assert.IsType<BadRequestResult>(new PostsController().Get("", "123", null).Result);
        }
        #endregion

    }
}
