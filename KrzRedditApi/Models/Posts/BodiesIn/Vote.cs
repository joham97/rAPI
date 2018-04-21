using RestApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditApi.BodiesIn
{
    public class Vote : JSONable
    {
        public Vote(int id, int value, int userId)
        {
            this.id = id;
            this.value = value;
            this.userId = userId;
        }
        
        public int id { get; private set; }
        public int value { get; private set; }
        public int userId { get; private set; }
    }
}
