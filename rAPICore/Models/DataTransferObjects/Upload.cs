using rAPI.Answers;

namespace rAPI.DTO
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
