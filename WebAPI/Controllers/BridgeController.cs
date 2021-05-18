using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;

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
        // public async Task<ActionResult<IEnumerable<Bridge>>> GetBridge()
        // {
        //     return await _context.Bridge.ToListAsync();
        // }
        public async Task<ObjectResult> GetBridge([FromQuery(Name = "page")] int page,
            [FromQuery(Name = "size")] int size)
        {
            page = page > 0 ? page - 1 : page;
            var result = new
            {
                message = "Lấy dữ liệu thành công",
                data = await _context.Bridge.Skip(page * size)
                    .Take(size).ToListAsync()
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
                await _context.SaveChangesAsync();
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
            _context.Bridge.Add(bridge);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBridge", new {id = bridge.Id}, bridge);
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
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BridgeExists(int id)
        {
            return _context.Bridge.Any(e => e.Id == id);
        }
    }
}