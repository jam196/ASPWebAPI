#nullable enable
using System;
using Newtonsoft.Json;

namespace WebAPI.Models
{
    public class User : BaseEntity
    {
        public int Id { get; set; }
        public string Username { get; set; }
        
        [JsonIgnore]
        public string Password { get; set; }
        
        public string? Role { get; set; }
        
        public string? Token { get; set; }
        // public DateTime CreatedAt { get; set; }
        // public DateTime UpdatedAt { get; set; }
        
        // public User()
        // {          
        //     this.CreatedAt  = DateTime.UtcNow;
        //     this.UpdatedAt = DateTime.UtcNow;
        // }
    }
}