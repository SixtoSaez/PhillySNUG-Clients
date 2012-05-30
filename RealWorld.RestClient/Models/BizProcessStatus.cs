using System.Collections.Generic;

namespace RealWorld.RestClient.Models
{
    public class BizProcessStatus
    {
        public string Status { get; set; }
        public IEnumerable<AppLink> ProcessingDetails { get; set; }
    }
}