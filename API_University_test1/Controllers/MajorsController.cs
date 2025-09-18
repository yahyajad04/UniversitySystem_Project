using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_University_test1.Data;
using API_University_test1.Models;

namespace API_University_test1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MajorsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MajorsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Majors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Majors>>> GetMajors()
        {
            return await _context.Majors.ToListAsync();
        }

        // GET: api/Majors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Majors>> GetMajors(int id)
        {
            var majors = await _context.Majors.FindAsync(id);

            if (majors == null)
            {
                return NotFound();
            }

            return majors;
        }

        // PUT: api/Majors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMajors(int id, Majors majors)
        {
            if (id != majors.Id)
            {
                return BadRequest();
            }

            _context.Entry(majors).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MajorsExists(id))
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

        // POST: api/Majors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Majors>> PostMajors(Majors majors)
        {
            _context.Majors.Add(majors);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMajors", new { id = majors.Id }, majors);
        }

        // DELETE: api/Majors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMajors(int id)
        {
            var majors = await _context.Majors.FindAsync(id);
            if (majors == null)
            {
                return NotFound();
            }

            _context.Majors.Remove(majors);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MajorsExists(int id)
        {
            return _context.Majors.Any(e => e.Id == id);
        }
    }
}
