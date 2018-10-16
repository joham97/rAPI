using rAPI.Answers;

namespace rAPI.DTO
{
    public class CreatePost : JSONable
    {
        public CreatePost(string title, string description, string path)
        {
            this.title = title;
            this.description = description;
            this.path = path;
        }
        
        public string title { get; private set; }
        public string description { get; private set; }
        public string path { get; private set; }
    }
}
