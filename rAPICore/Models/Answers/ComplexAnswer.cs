namespace rAPI.Answers
{
    public class ComplexAnswer : NormalAnswer
    {

        public ComplexAnswer(bool success, string message, int code, DataAnswer data) : base(success, message, code) {
            this.data = data;
        }

        public DataAnswer data { get; set; }

    }
}
