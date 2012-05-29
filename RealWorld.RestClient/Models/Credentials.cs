using System.Collections.Generic;

namespace RealWorld.RestClient.Models
{
    public class Credentials
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public IEnumerable<AppLink> Links { get; set; }
    }
}