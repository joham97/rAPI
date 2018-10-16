using System.Collections.Generic;

namespace rAPI.Answers
{
    public class ComplexListAnswer : NormalAnswer
    {

        public ComplexListAnswer(bool success, string message, int code, List<DataAnswer> data) : base(success, message, code) {
            this.data = data;
        }

        public List<DataAnswer> data { get; set; }

    }
}
