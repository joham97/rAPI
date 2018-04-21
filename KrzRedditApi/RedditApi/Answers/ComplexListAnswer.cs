using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditApi
{
    public class ComplexListAnswer : NormalAnswer
    {

        public ComplexListAnswer(bool success, string message, int code, List<DataAnswer> data) : base(success, message, code) {
            this.data = data;
        }

        public List<DataAnswer> data { get; set; }

    }
}
