using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditApi.BodiesOut
{
    public class Upload : DataAnswer
    {
        public Upload(string path)
        {
            this.path = path;
        }

        public string path { get; private set; }
    }
}
