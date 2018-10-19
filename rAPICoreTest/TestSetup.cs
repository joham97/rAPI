using Microsoft.AspNetCore.Mvc;
using rAPI.Answers;
using rAPI.Controllers;
using rAPI.DTO;
using rAPI.Services;

namespace rAPICoreTest
{
    class TestSetup
    {

        public static NormalAnswer NormalAnswer = new NormalAnswer(true, "successful", 200);

        public static LoginData TestLogin() {
            DatabaseService.Instance.ClearDatabase();

            RegisterController registerController = new RegisterController();
            Registration registation = new Registration("testusername", "testpassword");
            registerController.Post(registation);

            LoginController loginController = new LoginController();
            Login login = new Login("testusername", "testpassword");
            var loginResult = loginController.Post(login).Result;
            LoginData loginData = ((loginResult as OkObjectResult).Value as ComplexAnswer).data as LoginData;

            return loginData;
        }

    }
}
