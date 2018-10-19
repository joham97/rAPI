using Microsoft.AspNetCore.Mvc;
using rAPI.Answers;
using rAPI.Controllers;
using rAPI.DTO;
using rAPI.Services;
using Xunit;

namespace rAPICoreTest
{
    public class ValidateControllerTest
    {
        [Fact]
        public void Logout()
        {
            // Arrange
            DatabaseService.Instance.ClearDatabase();

            RegisterController registerController = new RegisterController();
            Registration registation = new Registration("testusername", "testpassword");
            registerController.Post(registation);

            LoginController loginController = new LoginController();
            Login login = new Login("testusername", "testpassword");
            var loginResult = loginController.Post(login).Result;
            string sessionkey = (((loginResult as OkObjectResult).Value as ComplexAnswer).data as LoginData).sessionkey;

            ValidateController validateController = new ValidateController();

            // Act
            var actionResult = validateController.Get(sessionkey).Result;

            // Assert            
            Assert.IsType<OkObjectResult>(actionResult);
        }

    }
}
