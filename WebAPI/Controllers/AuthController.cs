using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
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
            int statusCode = 200;

            try
            {
                user = _context.User.First(u => u.Username == userData.Username);
                if (passwordHash != user.Password)
                {
                    statusCode = 401;
                    status = "error";
                    message = "Bạn đã nhập sai mật khẩu, vui lòng thử lại!";
                }
            }
            catch (Exception e)
            {
                userData.Password = passwordHash;
                userData.Role = "member";
                userData.Avatar = "https://yt3.ggpht.com/Vai9EFHgVVYHlAax-zamzZUTqXV3pfBqxkHiMwafvtIwBDTTZfqKkiqoRmxT2I6bEJeL03AKgg=s1000-c-k-c0x00ffffff-no-rj";
                _context.User.Add(userData);
                _context.SaveChanges();

                message = "Tạo tài khoản thành công";

                user = userData;
            }

            if (status == "success")
            {
                History history = new History();
                if (Request.HttpContext.Connection.RemoteIpAddress != null)
                    history.IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                if (userData.Name != null)
                {
                    history.Content = "Thành viên " + userData.Name +" vừa đăng nhập vào hệ thống";
                }
                else
                {
                    history.Content = "Thành viên " + userData.Username +" vừa đăng nhập vào hệ thống";
                }
                _context.History.Add(history);
                _context.SaveChanges();
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("E465A3CCC379D80B29DFB0F2D30276E1");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new("id", user.Id.ToString()),
                    new("role", user.Role ?? "")
                }),
                Expires = DateTime.UtcNow.AddDays(365),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));

            var result = new
            {
                status,
                message,
                data = user,
                token
            };

            return StatusCode(statusCode, result);
        }

        // [HttpGet]
        // public async Task<ActionResult<IEnumerable<User>>> GetUser()
        // {
        //     return await _context.User.ToListAsync();
        // }
        [HttpGet("user")]
        public dynamic Test()
        {
            return HttpContext.Items["User"];
        }
        
        [HttpPost("logout")]
        public void logout()
        {
            
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