using System;
using System.Collections.Generic;
using System.Linq;
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
    public class BridgeController : ControllerBase
    {
        private readonly DbContext _context;

        public BridgeController(DbContext context)
        {
            _context = context;
        }

        // GET: api/Bridge
        [HttpGet]
        public async Task<ObjectResult> GetBridge([FromQuery(Name = "page")] int page,
            [FromQuery(Name = "size")] int size, [FromQuery(Name = "filters[0][value]")]
            string filter)
        {
            int totalRecords = await _context.Bridge.CountAsync();
            int lastPage = totalRecords / size;
            page = page > 0 ? page - 1 : page;
            List<Bridge> data;

            if (filter != null)
            {
                data = await _context.Bridge
                    .Where(s => s.Name.Contains(filter) || s.Investor.Contains(filter) || s.Designer.Contains(filter) ||
                                 s.Builder.Contains(filter) || s.Supervisor.Contains(filter) || s.Manager.Contains(filter)|| s.Location.Contains(filter)).OrderByDescending(s => s.Id)
                    .Skip(page * size)
                    .Take(size).ToListAsync();
            }
            else
            {
                data = await _context.Bridge.OrderByDescending(s => s.Id)
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

        // GET: api/Bridge/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Bridge>> GetBridge(int id)
        {
            var bridge = await _context.Bridge.FindAsync(id);

            if (bridge == null)
            {
                return NotFound();
            }

            return bridge;
        }

        // PUT: api/Bridge/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBridge(int id, Bridge bridge)
        {
            if (id != bridge.Id)
            {
                return BadRequest();
            }

            _context.Entry(bridge).State = EntityState.Modified;

            try
            {
                String message = "Cập nhật thông tin cầu " + bridge.Name + " thành công!";
                
                User userData = (User) HttpContext.Items["User"];
                History history = new History();
                if (Request.HttpContext.Connection.RemoteIpAddress != null)
                    history.IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                if (userData is {Name: { }})
                {
                    history.Content = "Thành viên " + userData.Name +" vừa cập nhật thông tin " + bridge.Name + " trong hệ thống";
                }
                else
                {
                    if (userData != null)
                        history.Content = "Thành viên " + userData.Username + " vừa cập nhật thông tin " + bridge.Name +
                                          " trong hệ thống";
                }
                _context.History.Add(history);
                await _context.SaveChangesAsync();
                
                var result = new
                {
                    message,
                    data = bridge
                };

                return StatusCode(200, result);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BridgeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Bridge
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Bridge>> PostBridge(Bridge bridge)
        {
            String message = "Thêm thành công " + bridge.Name + " vào hệ thống!";
            User userData = (User) HttpContext.Items["User"];
            if (userData != null)
            {
                bridge.UserId = userData.Id;
            }
            
            _context.Bridge.Add(bridge);
            
            History history = new History();
            if (Request.HttpContext.Connection.RemoteIpAddress != null)
                history.IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            if (userData is {Name: { }})
            {
                history.Content = "Thành viên " + userData.Name +" vừa thêm thành công " + bridge.Name + " vào hệ thống";
            }
            else
            {
                if (userData != null)
                    history.Content = "Thành viên " + userData.Username + " vừa thêm thành công " + bridge.Name +
                                      " vào hệ thống";
            }
            _context.History.Add(history);
            await _context.SaveChangesAsync();

            var result = new
            {
                message,
                data = bridge
            };

            return StatusCode(200, result);
        }


        // DELETE: api/Bridge/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBridge(int id)
        {
            var bridge = await _context.Bridge.FindAsync(id);
            if (bridge == null)
            {
                return NotFound();
            }

            _context.Bridge.Remove(bridge);
            
            User userData = (User) HttpContext.Items["User"];
            if (userData != null)
            {
                bridge.UserId = userData.Id;
            }

            History history = new History();
            if (Request.HttpContext.Connection.RemoteIpAddress != null)
                history.IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            if (userData is {Name: { }})
            {
                history.Content = "Thành viên " + userData.Name +" đã xóa cầu " + bridge.Name + " khỏi hệ thống";
            }
            else
            {
                if (userData != null)
                    history.Content = "Thành viên " + userData.Username + " đã xóa cầu " + bridge.Name +
                                      " khỏi hệ thống";
            }
            _context.History.Add(history);
            
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BridgeExists(int id)
        {
            return _context.Bridge.Any(e => e.Id == id);
        }
    }
}