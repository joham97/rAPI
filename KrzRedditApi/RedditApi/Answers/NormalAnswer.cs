using RestApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditApi {
    public class NormalAnswer : JSONable {
        public NormalAnswer(bool success, string message, int code) {
            this.success = success;
            this.message = message;
            this.code = code;
        }

        public bool success { get; private set; }
        public string message { get; private set; }
        public int code { get; private set; }
    }
}
