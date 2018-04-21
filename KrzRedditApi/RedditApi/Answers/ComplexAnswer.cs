using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditApi
{
    public class ComplexAnswer : NormalAnswer
    {

        public ComplexAnswer(bool success, string message, int code, DataAnswer data) : base(success, message, code) {
            this.data = data;
        }

        public DataAnswer data { get; set; }

    }
}
