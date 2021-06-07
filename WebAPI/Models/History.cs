using System.Net;

namespace WebAPI.Models
{
    public class History: BaseEntity
    {
        public int Id { get; set; }
        
        public string IpAddress { get; set; }
        public string Content { get; set; }
    }
}