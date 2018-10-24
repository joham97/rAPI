using Microsoft.AspNetCore.Mvc;
using rAPI.Controllers;
using Xunit;

namespace rAPICoreTest
{
    public class ForbiddenTest
    {

        [Fact]
        public void ForbiddenTests()
        {
            Assert.IsType<ForbidResult>(new LoginController().Get());
            Assert.IsType<ForbidResult>(new LoginController().Put());
            Assert.IsType<ForbidResult>(new LoginController().Delete());

            Assert.IsType<ForbidResult>(new LogoutController().Post());
            Assert.IsType<ForbidResult>(new LogoutController().Put());
            Assert.IsType<ForbidResult>(new LogoutController().Delete());

            Assert.IsType<ForbidResult>(new UserController().Post());
            Assert.IsType<ForbidResult>(new UserController().Put());
            Assert.IsType<ForbidResult>(new UserController().Delete());

            Assert.IsType<ForbidResult>(new ValidateController().Post());
            Assert.IsType<ForbidResult>(new ValidateController().Put());
            Assert.IsType<ForbidResult>(new ValidateController().Delete());

            Assert.IsType<ForbidResult>(new RegisterController().Get());
            Assert.IsType<ForbidResult>(new RegisterController().Put());
            Assert.IsType<ForbidResult>(new RegisterController().Delete());

            Assert.IsType<ForbidResult>(new PostController().Put());

            Assert.IsType<ForbidResult>(new PostsController().Post());
            Assert.IsType<ForbidResult>(new PostsController().Put());
            Assert.IsType<ForbidResult>(new PostsController().Delete());

            Assert.IsType<ForbidResult>(new UploadController().Get());
            Assert.IsType<ForbidResult>(new UploadController().Put());
            Assert.IsType<ForbidResult>(new UploadController().Delete());

            Assert.IsType<ForbidResult>(new VoteController().Get());
            Assert.IsType<ForbidResult>(new VoteController().Put());
            Assert.IsType<ForbidResult>(new VoteController().Delete());

            Assert.IsType<ForbidResult>(new CommentController().Get());
            Assert.IsType<ForbidResult>(new CommentController().Put());
            Assert.IsType<ForbidResult>(new CommentController().Delete());

            Assert.IsType<ForbidResult>(new VoteCommentController().Get());
            Assert.IsType<ForbidResult>(new VoteCommentController().Put());
            Assert.IsType<ForbidResult>(new VoteCommentController().Delete());
        }

    }
}
