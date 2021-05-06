using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebAPI.Helpers;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DbContext _context;
        public AuthController(DbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public dynamic Login(User userData)
        {
            User user;
            String passwordHash = EncodePassword(userData.Password);
            var status = "success";
            var message = "Đăng nhập thành công";

            try
            {
                user = _context.User.First(u => u.Username == userData.Username);
                if (passwordHash != user.Password)
                {
                    status = "error";
                    message = "Sai mật khẩu";
                }
            }
            catch (Exception e)
            {
                userData.Password = passwordHash;
                userData.Role = "member";
                _context.User.Add(userData);
                _context.SaveChanges();

                message = "Tạo tài khoản thành công";

                user = userData;
            }
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("E465A3CCC379D80B29DFB0F2D30276E1");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] 
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role ?? "")
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            var result = new
            {
                status,
                message,
                data = user
            };

            return Ok(result);
        }

        // [HttpGet]
        // public async Task<ActionResult<IEnumerable<User>>> GetUser()
        // {
        //     return await _context.User.ToListAsync();
        // }
        [HttpGet("test")]
        public String Test()
        {
            return "result";
        }

        public static string EncodePassword(string originalPassword)
        {
            // //Declarations
            Byte[] originalBytes;
            Byte[] encodedBytes;
            MD5 md5;

            //Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)
            md5 = new MD5CryptoServiceProvider();
            originalBytes = Encoding.Default.GetBytes(originalPassword);
            encodedBytes = md5.ComputeHash(originalBytes);

            //Convert encoded bytes back to a 'readable' string
            return BitConverter.ToString(encodedBytes).Replace("-", "");
            ;
        }
    }
}