using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;
using Microsoft.AspNetCore.HttpOverrides;

namespace WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DbContext _context;

        public UserController(DbContext context)
        {
            _context = context;
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUser([FromQuery(Name = "page")] int page,
            [FromQuery(Name = "size")] int size, [FromQuery(Name = "filters[0][value]")]
            string filter)
        {
            int totalRecords = await _context.User.CountAsync();
            int lastPage = totalRecords / size;
            page = page > 0 ? page - 1 : page;
            List<User> data;

            if (filter != null)
            {
                data = await _context.User
                    .Where(s => s.Username.Contains(filter)).OrderByDescending(s => s.Id)
                    .Skip(page * size)
                    .Take(size).ToListAsync();
            }
            else
            {
                data = await _context.User.OrderByDescending(s => s.Id)
                    .Skip(page * size)
                    .Take(size).ToListAsync();
            }

            var result = new
            {
                message = "Lấy dữ liệu thành công",
                last_page = lastPage,
                data
            };

            return StatusCode(200, result);
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/User/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            String message = null;
            int status = 200;
            object result;
            
            User userData = (User) HttpContext.Items["User"];
            if (userData != null && userData.Id != user.Id && userData.Role != "admin")
            {
                message = "Bạn không có quyền thay đổi thông tin thành viên này";
                status = 505;
            }

            if (status == 200)
            {
                message = "Cập nhật thông tin thành viên " + user.Username + " thành công!";
                
                if (userData != null && userData.Password != user.Password)
                {
                    user.Password = EncodePassword(user.Password);;
                    message = "Đổi mật khẩu thành viên " + user.Username + " thành công!";
                }
                
                _context.Entry(user).State = EntityState.Modified;
                try
                {
                    History history = new History();
                    if (Request.HttpContext.Connection.RemoteIpAddress != null)
                        history.IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                    history.Content = "Admin vừa cập nhật thông tin thành viên " + user.Username + " trong hệ thống";
                    _context.History.Add(history);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            
            result = new
            {
                message,
                data = user
            };

            return StatusCode(status, result);

            // return NoContent();
        }

        // POST: api/User
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var user = await _context.User.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                User userData = (User) HttpContext.Items["User"];
                if (userData != null && userData.Id != user.Id && userData.Role != "admin")
                {
                    return StatusCode(505, new
                    {
                        message = "Bạn không có quyền xóa thành viên này"
                    });
                }

                _context.User.Remove(user);

                History history = new History();
                if (Request.HttpContext.Connection.RemoteIpAddress != null)
                    history.IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                history.Content = "Admin đã xóa thành viên " + user.Username + " khỏi hệ thống";
                _context.History.Add(history);

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException)
            {
                return StatusCode(505, new
                {
                    message = "Thông tin thành viên này liên kết với một bản ghi khác trong hệ thống, không thể xóa!"
                });
            }
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
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
