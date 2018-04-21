using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestApi
{
    public class Request
    {
        public string Pfad { get; set; }
        public RequestType Type { get; set; }
        public string Content { get; set; }
        public Dictionary<string, string> Data { get; set; }
        public string File { get; set; }
        public string FileName { get; set; }
    }

    public enum RequestType
    {
        GET,
        POST,
        PUT,
        DELETE,
        OPTIONS
    }
}
