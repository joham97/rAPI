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
        public void Validate()
        {
            // Arrange
            LoginData loginData = TestSetup.TestLogin();

            ValidateController validateController = new ValidateController();

            // Act
            var actionResult = validateController.Get(loginData.sessionkey).Result;

            // Assert            
            Assert.IsType<OkObjectResult>(actionResult);
        }

    }
}
