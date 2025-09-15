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
    public class CoursesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CoursesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Courses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Studentdto>>> GetCourses()
        {
            var courses = await _context.Courses
                .Include(s => s.Students)
                .ToListAsync();

            List<Coursedto> dto_list = new List<Coursedto>();
            foreach (var course in courses)
            {
                dto_list.Add(new Coursedto
                {
                    Id = course.Id,
                    Course_Name = course.Course_Name,
                    Teacher_Name = course.Teacher_Name,
                    Course_Hours = course.Course_Hours,
                    Students = course.Students.Select(c => new Studentdto
                    {
                        Id = c.Id,
                        Name = c.Name
                    }).ToList()
                });
            }

            return Ok(dto_list);

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Students>> GetCourse(int id)
        {
            {
                var course = await _context.Courses
                    .Include(s => s.Students)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (course == null) return NotFound();

                var dto = new Coursedto
                {
                    Id = course.Id,
                    Course_Name = course.Course_Name,
                    Students = course.Students.Select(c => new Studentdto
                    {
                        Id = c.Id,
                        Name = c.Name
                    }).ToList()
                };

                return Ok(dto);
            }
        }


        // PUT: api/Courses/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCourses(int id, Courses courses)
        {
            if (id != courses.Id)
            {
                return BadRequest();
            }

            _context.Entry(courses).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CoursesExists(id))
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

        // POST: api/Courses
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Courses>> PostCourses(Courses courses)
        {
            _context.Courses.Add(courses);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCourses", new { id = courses.Id }, courses);
        }
            // DELETE: api/Courses/5
            [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourses(int id)
        {
            var courses = await _context.Courses.FindAsync(id);
            if (courses == null)
            {
                return NotFound();
            }

            _context.Courses.Remove(courses);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CoursesExists(int id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }
    }
}
