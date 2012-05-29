using System.Collections.Generic;

namespace RealWorld.RestClient.Models
{
    public class BizProcessStatus
    {
        public string Status { get; set; }
        public IEnumerable<string> ProcessingDetails { get; set; }
    }
}