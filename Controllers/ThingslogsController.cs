using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Data.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThingslogsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ThingslogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Thingslogs
        [Authorize(Roles="viewer")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Thingslog>>> GetThingslog()
        {            
            var groupList = new List<string>();
            ClaimsPrincipal principal = HttpContext.User as ClaimsPrincipal;
            if (null != principal)
            {
                foreach (Claim claim in principal.Claims)
                {
                    Console.WriteLine("TYPE: " + claim.Type + "; VALUE: " + claim.Value);
                    if (claim.Type == "groups")
                    {
                        groupList.Add(claim.Value);
                    }
                }
            }

            var thingslogs = await _context.Thingslog
                 .Where(p => groupList.Contains(p.Tenant))
                 .ToListAsync();

            return thingslogs.ToArray();
        }

        // GET: api/Thingslogs/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Thingslog>> GetThingslog(int id)
        {
            var thingslog = await _context.Thingslog.FindAsync(id);

            if (thingslog == null)
            {
                return NotFound();
            }

            return thingslog;
        }

        // PUT: api/Thingslogs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutThingslog(int id, Thingslog thingslog)
        {
            if (id != thingslog.Id)
            {
                return BadRequest();
            }

            _context.Entry(thingslog).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ThingslogExists(id))
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

        // POST: api/Thingslogs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Thingslog>> PostThingslog(Thingslog thingslog)
        {
            _context.Thingslog.Add(thingslog);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetThingslog", new { id = thingslog.Id }, thingslog);
        }

        // DELETE: api/Thingslogs/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteThingslog(int id)
        {
            var thingslog = await _context.Thingslog.FindAsync(id);
            if (thingslog == null)
            {
                return NotFound();
            }

            _context.Thingslog.Remove(thingslog);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ThingslogExists(int id)
        {
            return _context.Thingslog.Any(e => e.Id == id);
        }
    }
}
