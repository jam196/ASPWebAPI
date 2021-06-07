#nullable enable
using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace WebAPI.Models
{
    public class User : BaseEntity
    {
        public int Id { get; set; }
        
        public string? Name { get; set; }
        
        public string? Avatar { get; set; }
        
        [Required(ErrorMessage = "Username không được bỏ trống")]
        public string Username { get; set; }
        
        [JsonIgnore]
        [Required(ErrorMessage = "Password không được bỏ trống")]
        public string Password { get; set; }
        
        public string? Role { get; set; }
    }
}