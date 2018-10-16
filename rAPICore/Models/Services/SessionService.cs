using rAPI.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace rAPI.Services
{
    public class SessionService : Dictionary<string, Session>
    {

        public static readonly SessionService Instance = new SessionService();

        private SessionService() : base() { }

        public static string GenerateSessionKey() {
            byte[] array = new byte[64];
            Random random = new Random();
            random.NextBytes(array);
            string GuidString = Convert.ToBase64String(array);
            return GuidString.Replace("/", "").Replace("+", "");
        }

    }
}