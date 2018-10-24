using Microsoft.AspNetCore.Mvc;
using rAPI.Controllers;
using rAPI.DTO;
using rAPI.Services;
using Xunit;

namespace rAPICoreTest
{
    public class RegisterControllerTest
    {
        [Fact]
        public void Register()
        {
            // Arrange
            DatabaseService.Instance.ClearDatabase();

            RegisterController registerController = new RegisterController();
            Registration registation = new Registration("testusername", "testpassword");

            // Act
            var actionResult = registerController.Post(registation).Result;

            // Assert            
            Assert.IsType<OkObjectResult>(actionResult);
        }

        [Fact]
        public void RegisterTwice()
        {
            // Arrange
            DatabaseService.Instance.ClearDatabase();

            RegisterController registerController = new RegisterController();
            Registration registation = new Registration("testusername", "testpassword");
            registerController.Post(registation);

            // Act
            var actionResult = registerController.Post(registation).Result;

            // Assert            
            Assert.IsType<ConflictObjectResult>(actionResult);
        }
    }
}
