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
    public class StatisticController : ControllerBase
    {
        private readonly DbContext _context;

        public StatisticController(DbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ObjectResult> Index()
        {
            // <List<Bridge>> bridges = _context.Bridge.ToListAsync();
            int bridgeCount = await _context.Bridge.CountAsync();
            int userCount = await _context.User.CountAsync();
            int bridgeGoodCount = await _context.Bridge.Where(s => s.Status.Contains("good")).CountAsync();
            int bridgeWarningCount = await _context.Bridge.Where(s => s.Status.Contains("warning")).CountAsync();
            int bridgeBadCount = await _context.Bridge.Where(s => s.Status.Contains("bad")).CountAsync();
            
            List<History> recentlyHistories = await _context.History.OrderByDescending(s => s.Id).Take(5).ToListAsync();
            List<Bridge> badBridges = await _context.Bridge.OrderByDescending(s => s.Id).Where(s => s.Status.Contains("bad")).Take(7).ToListAsync();

            object data = new
            {
                bridgeCount,
                userCount,
                bridgeGoodCount,
                bridgeWarningCount,
                bridgeBadCount,
                recentlyHistories,
                badBridges
            };

            object result = new
            {
                message = "Lấy dữ liệu thành công",
                data
            };

            return StatusCode(200, result);
        }
    }
}