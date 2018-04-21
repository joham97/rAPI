using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditApiTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //DatabaseTest.Test();
            RedditApiTest test = new RedditApiTest();
            test.TestSession();
            test.TestPosts();
        }
    }
}
