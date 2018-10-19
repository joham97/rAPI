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
            DatabaseService.Instance.ClearDatabase();

            RegisterController registerController = new RegisterController();
            Registration registation = new Registration("testusername", "testpassword");
            registerController.Post(registation);

            LoginController loginController = new LoginController();
            Login login = new Login("testusername", "testpassword");
            var loginResult = loginController.Post(login).Result;
            int userId = (((loginResult as OkObjectResult).Value as ComplexAnswer).data as LoginData).userId;
                        
            UserController userController = new UserController();
            var expected = new UserData(1, "testusername");

            // Act
            var actionResult = userController.Get(userId).Result;
            var actual = (((actionResult as OkObjectResult).Value as ComplexAnswer).data as UserData);

            // Assert            
            Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(expected.userId, actual.userId);
            Assert.Equal(expected.username, actual.username);
        }

    }
}
