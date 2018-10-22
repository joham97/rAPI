using Microsoft.AspNetCore.Mvc;
using rAPI.Answers;
using rAPI.Controllers;
using rAPI.DTO;
using rAPI.Services;
using System.Collections.Generic;
using Xunit;

namespace rAPICoreTest
{
    public class CommentControllerTest
    {
        

        public LoginData CreatePost()
        {
            // Arrange
            LoginData loginData = TestSetup.TestLogin();

            PostController postController = new PostController();

            // Act
            var actionResult = postController.Post(loginData.sessionkey, Mocks.post).Result;
            var actual = (actionResult as OkObjectResult).Value as NormalAnswer;

            // Assert            
            Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(TestSetup.NormalAnswer.code, actual.code);
            Assert.Equal(TestSetup.NormalAnswer.message, actual.message);
            Assert.Equal(TestSetup.NormalAnswer.success, actual.success);

            return loginData;
        }

        [Fact]
        public LoginData CreateComment()
        {
            // Arrange
            LoginData loginData = CreatePost();

            CommentController commentController = new CommentController();

            // Act
            var actionResult = commentController.Post(loginData.sessionkey, Mocks.comment1).Result;
            var actual = (actionResult as OkObjectResult).Value as NormalAnswer;

            // Assert            
            Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(TestSetup.NormalAnswer.code, actual.code);
            Assert.Equal(TestSetup.NormalAnswer.message, actual.message);
            Assert.Equal(TestSetup.NormalAnswer.success, actual.success);

            return loginData;
        }

        [Fact]
        public void CreateVoteComment()
        {
            // Arrange
            LoginData loginData = CreateComment();

            VoteCommentController voteCommentController = new VoteCommentController();

            // Act
            var actionResult = voteCommentController.Post(loginData.sessionkey, Mocks.voteOnCommentPositive).Result;
            var actual = ((actionResult as OkObjectResult).Value as ComplexAnswer);
            var data = actual.data as Comment;

            // Assert            
            Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(TestSetup.NormalAnswer.code, actual.code);
            Assert.Equal(TestSetup.NormalAnswer.message, actual.message);
            Assert.Equal(TestSetup.NormalAnswer.success, actual.success);
            Assert.Equal(1, data.yourvote);
        }

        [Fact]
        public void CreateNegativeVoteComment()
        {
            // Arrange
            LoginData loginData = CreateComment();

            VoteCommentController voteCommentController = new VoteCommentController();

            // Act
            var actionResult = voteCommentController.Post(loginData.sessionkey, Mocks.voteOnCommentNegative).Result;
            var actual = ((actionResult as OkObjectResult).Value as ComplexAnswer);
            var data = actual.data as Comment;

            // Assert            
            Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(TestSetup.NormalAnswer.code, actual.code);
            Assert.Equal(TestSetup.NormalAnswer.message, actual.message);
            Assert.Equal(TestSetup.NormalAnswer.success, actual.success);
            Assert.Equal(-1, data.yourvote);
        }

        [Fact]
        public void CreateSubComment()
        {
            // Arrange
            LoginData loginData = CreatePost();

            CommentController commentController = new CommentController();

            // Act
            commentController.Post(loginData.sessionkey, Mocks.comment1);
            var actionResult = commentController.Post(loginData.sessionkey, Mocks.subComment).Result;
            var actual = (actionResult as OkObjectResult).Value as ComplexAnswer;
            var data = actual.data as Comment;

            // Assert            
            Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(TestSetup.NormalAnswer.code, actual.code);
            Assert.Equal(TestSetup.NormalAnswer.message, actual.message);
            Assert.Equal(TestSetup.NormalAnswer.success, actual.success);
            Assert.Equal(Mocks.subComment.text, data.text);
        }

        [Fact]
        public void CreateCommentFailed()
        {
            Assert.IsType<UnauthorizedResult>(new CommentController().Post("", Mocks.comment1).Result);

            LoginData loginData = TestSetup.TestLogin();
            Assert.IsType<NotFoundResult>(new CommentController().Post(loginData.sessionkey, Mocks.comment1).Result);
        }

        [Fact]
        public void VoteCommentFailed()
        {
            Assert.IsType<UnauthorizedResult>(new VoteCommentController().Post("", Mocks.voteOnCommentPositive).Result);

            LoginData loginData = TestSetup.TestLogin();
            Assert.IsType<NotFoundResult>(new VoteCommentController().Post(loginData.sessionkey, new CommentVote(123, 1)).Result);
        }

    }
}
