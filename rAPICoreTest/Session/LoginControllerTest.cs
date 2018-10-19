using Microsoft.AspNetCore.Mvc;
using rAPI.Controllers;
using rAPI.DTO;
using rAPI.Services;
using Xunit;

namespace rAPICoreTest
{
    public class LoginControllerTest
    {
        [Fact]
        public void Login()
        {
            // Arrange
            DatabaseService.Instance.ClearDatabase();

            RegisterController registerController = new RegisterController();
            Registration registation = new Registration("testusername", "testpassword");
            registerController.Post(registation);

            LoginController loginController = new LoginController();
            Login login = new Login("testusername", "testpassword");

            // Act
            var actionResult = loginController.Post(login).Result;

            // Assert            
            Assert.IsType<OkObjectResult>(actionResult);
        }
    }
}
