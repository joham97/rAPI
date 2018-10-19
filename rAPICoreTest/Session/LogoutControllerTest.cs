using Microsoft.AspNetCore.Mvc;
using rAPI.Answers;
using rAPI.Controllers;
using rAPI.DTO;
using rAPI.Services;
using Xunit;

namespace rAPICoreTest
{
    public class LogoutControllerTest
    {
        [Fact]
        public void Logout()
        {
            // Arrange
            LoginData loginData = TestSetup.TestLogin();

            LogoutController logoutController = new LogoutController();

            // Act
            var actionResult = logoutController.Get(loginData.sessionkey).Result;

            // Assert            
            Assert.IsType<OkObjectResult>(actionResult);
        }

    }
}
