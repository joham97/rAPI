namespace rAPI.Answers
{
    public abstract class JSONable
    {
        public string ToJSON()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
        }
    }
}
