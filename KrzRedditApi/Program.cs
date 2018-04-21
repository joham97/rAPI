using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditApi {
    class Program {
        static void Main(string[] args) {
            RedditApiController api = new RedditApiController(8081);
            api.Start();
        }
    }
}
