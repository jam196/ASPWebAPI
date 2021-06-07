using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoryController : ControllerBase
    {
        private readonly DbContext _context;

        public HistoryController(DbContext context)
        {
            _context = context;
        }

        // GET: api/History
        [HttpGet]
        public async Task<ActionResult<IEnumerable<History>>> GetHistory([FromQuery(Name = "page")] int page,
            [FromQuery(Name = "size")] int size, [FromQuery(Name = "filters[0][value]")]
            string filter)
        {
            int totalRecords = await _context.Bridge.CountAsync();
            int lastPage = totalRecords / size;
            page = page > 0 ? page - 1 : page;
            List<History> data;
            
            if (filter != null)
            {
                data = await _context.History
                    .Where(s => s.Content.Contains(filter) || s.IpAddress.Contains(filter)).OrderByDescending(s => s.Id)
                    .Skip(page * size)
                    .Take(size).ToListAsync();
            }
            else
            {
                data = await _context.History.OrderByDescending(s => s.Id)
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

        // GET: api/History/5
        [HttpGet("{id}")]
        public async Task<ActionResult<History>> GetHistory(int id)
        {
            var history = await _context.History.FindAsync(id);

            if (history == null)
            {
                return NotFound();
            }

            return history;
        }

        // PUT: api/History/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHistory(int id, History history)
        {
            if (id != history.Id)
            {
                return BadRequest();
            }

            _context.Entry(history).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HistoryExists(id))
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

        // POST: api/History
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<History>> PostHistory(History history)
        {
            _context.History.Add(history);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetHistory", new { id = history.Id }, history);
        }

        // DELETE: api/History/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHistory(int id)
        {
            var history = await _context.History.FindAsync(id);
            if (history == null)
            {
                return NotFound();
            }

            _context.History.Remove(history);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool HistoryExists(int id)
        {
            return _context.History.Any(e => e.Id == id);
        }
    }
}
