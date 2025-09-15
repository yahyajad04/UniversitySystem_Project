using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_University_test1.Data;
using API_University_test1.Models;
using Microsoft.AspNetCore.Identity;

namespace API_University_test1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public StudentsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Students
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Students>>> GetStudents()
        {
            var students = await _context.Students
                .Include(s => s.courses)
                .Include(s => s.Major)
                .ToListAsync();

            List<Students> students_list = new List<Students>();
            foreach (var student in students)
            {
                students_list.Add(new Students
                {
                    Id = student.Id,
                    Name = student.Name,
                    Email= student.Email,
                    Phone = student.Phone,
                    Major = student.Major,
                    hours_term = student.hours_term,
                    reciept = student.reciept,
                    UserId = student.UserId,
                    courses = student.courses.Select(c => new Courses
                    {
                        Id = c.Id,
                        Course_Name = c.Course_Name,
                        Teacher_Name = c.Teacher_Name,
                        Course_Hours = c.Course_Hours
                    }).ToList()
                });
            }

                return Ok(students_list);
           
        }

        // GET: api/Students/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Students>> GetStudents(int id)
        {
            {
                var student = await _context.Students
                    .Include(s => s.courses)
                    .Include(s => s.Major)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (student == null) return NotFound();

                var dto = new Students
                {
                    Id = student.Id,
                    Name = student.Name,
                    Email = student.Email,
                    Phone = student.Phone,
                    Major = student.Major,
                    hours_term = student.hours_term,
                    reciept = student.reciept,
                    UserId = student.UserId,
                    courses = student.courses.Select(c => new Courses
                    {
                        Id = c.Id,
                        Course_Name = c.Course_Name,
                        Teacher_Name = c.Teacher_Name,
                        Course_Hours = c.Course_Hours
                    }).ToList()
                };

                return Ok(dto);
            }
        }
        [HttpGet("byUser/{userId}")]
        public async Task<ActionResult<Studentdto>> GetStudentByUserId(string userId)
        {
            var student = await _context.Students
                .Include(s => s.courses)
                .Include(s => s.Major)
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (student == null)
                return NotFound();

            var dto = new Studentdto
            {
                Id = student.Id,
                Name = student.Name,
                Email = student.Email,
                UserId = student.UserId,
                Major = student.Major,
                reciept = student.reciept,
                hours_term = student.hours_term,
                Courses = student.courses.Select(c => new Coursedto
                {
                    Id = c.Id,
                    Course_Name = c.Course_Name,
                    Teacher_Name = c.Teacher_Name,
                    Course_Hours = c.Course_Hours
                }).ToList()
            };

            return Ok(dto);
        }

        // PUT: api/Students/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudents(int id, Students students)
        {
            if (id != students.Id)
            {
                return BadRequest();
            }

            _context.Entry(students).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentsExists(id))
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

        [HttpPut("{id}/reciept")]
        public async Task<IActionResult> PutStudentsreciept(int id, int hours_term,int reciept)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            //_context.Entry(student).State = EntityState.Modified;
            student.hours_term = hours_term;
            student.reciept = reciept;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentsExists(id))
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

        [HttpPut("{id}/Admin")]
        public async Task<IActionResult> PutStudentsAdmin(int id, string Name, string Email,int hours_term, int reciept)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            //_context.Entry(student).State = EntityState.Modified;
            student.Name = Name;
            student.Email = Email;
            student.hours_term = hours_term;
            student.reciept = reciept;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentsExists(id))
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
        [HttpPut("{id}/Profile")]
        public async Task<IActionResult> PutStudentsProfile(int id, string Name, string Email, string Phone)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            //_context.Entry(student).State = EntityState.Modified;
            student.Name = Name;
            student.Email = Email;
            student.Phone = Phone;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentsExists(id))
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
        // POST: api/Students
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Students>> PostStudents(Students students)
        {
                _context.Students.Add(students);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetStudents", new { id = students.Id }, students); 
        }

        [HttpPost("{StudentId}/courses")]
        public async Task<ActionResult<Courses>> EnrollCourse(int StudentId, [FromForm] int courseId)
        {
            var student = await _context.Students
                .Include(s => s.courses)
                .Include(s => s.Major)
                .FirstOrDefaultAsync(s => s.Id == StudentId);
            if (student == null)
            {
                return NotFound("The student doesnt exist");
            }
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
            {
                return NotFound("The course doesnt exist");
            }
            if (student.courses.Any(c => c.Id == courseId))
            {
                return BadRequest("this course already enrolled");
            }

            student.courses.Add(course);
            await _context.SaveChangesAsync();
            return Ok("Added Successfully");
        }
        [HttpPost("{StudentId}/courses/unroll")]
        public async Task<ActionResult<Courses>> UnrollCourse(int StudentId, [FromForm] int courseId)
        {
            var student = await _context.Students
                .Include(s => s.courses)
                .Include(s => s.Major)
                .FirstOrDefaultAsync(s => s.Id == StudentId);
            if (student == null)
            {
                return NotFound("The student doesnt exist");
            }
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
            {
                return NotFound("The course doesnt exist");
            }
            if (student.courses.Any(c => c.Id == courseId))
            {

                student.courses.Remove(course);
                await _context.SaveChangesAsync();
                return Ok("course removed successfully");  
            }

            return BadRequest("this course is not enrolled");
        }

        // DELETE: api/Students/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudents(int id)
        {
            var students = await _context.Students.FindAsync(id);
            if (students == null)
            {
                return NotFound();
            }

            _context.Students.Remove(students);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StudentsExists(int id)
        {
            return _context.Students.Any(e => e.Id == id);
        }
    }
}
