using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;
using RedditApi;
using RedditApi.BodiesIn;
using RedditApi.BodiesOut;
using RestApi;

namespace RedditApiTest
{
    class RedditApiTest
    {

        private RestApiTest apiTest;

        public RedditApiTest()
        {
            File.Delete("reddit.db");
            RedditApiController api = new RedditApiController(8081);
            new Thread(new ThreadStart(() =>
            {
                api.Start();
            })).Start();
            this.apiTest = new RestApiTest("http://localhost:8081");
            Thread.Sleep(500);
        }

        public void TestSession()
        {
            Console.WriteLine();
            Console.WriteLine("Session Tests:");

            GetTest("/test", null);
            GetTest("/register?username=jonas&password=jonas", null);
            var login = GetTest("/login?username=jonas&password=jonas", null);
            var sessionkey = login.Root.Element("data").Element("sessionkey").Value;
            var sessionkeyDictionary = new Dictionary<string, string>() { { "sessionkey", sessionkey } };
            GetTest("/validate", sessionkeyDictionary);
            GetTest("/logout", sessionkeyDictionary);
            GetTest("/validate", sessionkeyDictionary);
        }

        public void TestPosts()
        {
            Console.WriteLine();
            Console.WriteLine("Post Tests:");

            var login = GetTest("/login?username=jonas&password=jonas", null);
            var sessionkey = login.Root.Element("data").Element("sessionkey").Value;
            var userId = login.Root.Element("data").Element("userId").Value;

            var sessionkeyDictionary = new Dictionary<string, string>() { { "sessionkey", sessionkey } };

            var post = new CreatePost("Testbeitrag", "Testbeschreibung", "");
            PostTest("/post", sessionkeyDictionary, post);
            GetTest("/post", new Dictionary<string, string>() { { "postId", "1" } });
            GetTest("/post", new Dictionary<string, string>() { { "postId", "1" }, { "sessionkey", sessionkey } });
            GetTest("/post/new", null);
            GetTest("/post/hot", null);
            GetTest("/post/new", sessionkeyDictionary);
            GetTest("/post/hot", sessionkeyDictionary);

            var voteDictionary = new Dictionary<string, string>() {
                { "sessionkey", sessionkey },
                { "postId", "1" },
                { "value", "1" }
            };
            PutTest("/post/vote", voteDictionary, null);
        }

        private XDocument GetTest(string path, Dictionary<string, string> parameters)
        {
            return RestTest(RequestType.GET, path, parameters, null);
        }

        private XDocument PostTest(string path, Dictionary<string, string> parameters, object data)
        {
            return RestTest(RequestType.POST, path, parameters, data);
        }

        private XDocument PutTest(string path, Dictionary<string, string> parameters, object data)
        {
            return RestTest(RequestType.PUT, path, parameters, data);
        }

        private XDocument RestTest(RequestType type, string path, Dictionary<string, string> parameters, object data)
        {
            string success = "false";
            string message = "Something went wrong!";
            XDocument testResult = null;
            try
            {
                Task<string> testTask = null;
                if (type == RequestType.GET)
                    testTask = Task.Run(() => apiTest.Get(path, parameters));
                else if (type == RequestType.POST)
                    testTask = Task.Run(() => apiTest.Post(path, parameters, data));
                else if (type == RequestType.PUT)
                    testTask = Task.Run(() => apiTest.Put(path, parameters, data));

                testTask.Wait();
                testResult = JsonConvert.DeserializeXNode("{ data: " + testTask.Result + " }");
                success = testResult.Root.Element("success").Value;
                message = testResult.Root.Element("message").Value;
            }
            catch (Exception e)
            {
                message = e.Message;
            }
            string typeString = "";
            if (type == RequestType.GET)
                typeString = "GET";
            else if (type == RequestType.POST)
                typeString = "POST";
            else if (type == RequestType.PUT)
                typeString = "PUT";

            ShowResult(typeString, path, success, message);
            return testResult;
        }

        private void ShowResult(string request_type, string path, string success, string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{request_type} ");
            Console.ForegroundColor = (success.Equals("true")) ? ConsoleColor.Green : ConsoleColor.Red;
            Console.Write($"{success} ");
            Console.ResetColor();
            Console.WriteLine($"{path}: ({message})");
        }
    }
}
