using Microsoft.AspNetCore.Mvc;
using rAPI.Answers;
using rAPI.Controllers;
using rAPI.DTO;
using rAPI.Services;
using Xunit;

namespace rAPICoreTest
{
    public class UserControllerTest
    {
        [Fact]
        public void UserData()
        {
            // Arrange
            LoginData loginData = TestSetup.TestLogin();

            UserController userController = new UserController();
            var expected = new UserData(1, "testusername");

            // Act
            var actionResult = userController.Get(loginData.userId).Result;
            var actual = (((actionResult as OkObjectResult).Value as ComplexAnswer).data as UserData);

            // Assert            
            Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(expected.userId, actual.userId);
            Assert.Equal(expected.username, actual.username);
        }
        [Fact]
        public void UserDataFailed()
        {
            Assert.IsType<NotFoundObjectResult>(new UserController().Get(123).Result);
        }
    }
}
