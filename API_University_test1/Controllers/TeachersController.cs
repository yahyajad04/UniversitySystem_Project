using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_University_test1.Data;
using API_University_test1.Models;
using API_University_test1.Models.DTO_s;

namespace API_University_test1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeachersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TeachersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Teachers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Teachers>>> GetTeachers()
        {
            return await _context.Teachers
                .Include(t => t.T_courses)
                .ThenInclude(c => c.Students)
                .ThenInclude(s => s.Grades)
                .ToListAsync();
        }

        // GET: api/Teachers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Teachers>> GetTeachers(int id)
        {
            var teachers = await _context.Teachers.FindAsync(id);

            if (teachers == null)
            {
                return NotFound();
            }

            return teachers;
        }

        [HttpGet("byUser/{userId}")]
        public async Task<ActionResult<Teachers>> GetTeacherByUserId(string userId)
        {
            var teacher = await _context.Teachers
                .Include(t => t.T_courses)
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (teacher == null)
                return NotFound();

            var dto = new Teacherdto
            {
                Id = teacher.Id,
                UserId = userId,
                Teacher_Name = teacher.Teacher_Name,
                Email = teacher.Email,
                T_courses = teacher.T_courses.Select(c => new Courses
                {
                    Id = c.Id,
                    Course_Name = c.Course_Name,
                    Description = c.Description,
                    Course_Hours = c.Course_Hours,
                    isDone = c.isDone,
                    isApproved = c.isApproved,
                    Students = c.Students.Select(s => new Students
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Email = s.Email,
                        Grades = s.Grades.Select(g => new Grades
                        {
                            CourseId = g.Id,
                            first = g.first,
                            second = g.second,
                            final = g.final,
                            total = g.total
                        }).ToList(),
                    }).ToList(),
                }).ToList()
            };

            return Ok(dto);
        }


        [HttpGet("Dto")]
        public async Task<ActionResult<Teacherdto>> GetTeacherByDto()
        {
            var teachers = await _context.Teachers
                .Include(t => t.T_courses)
                .ThenInclude(c => c.Students)
                .ThenInclude(s => s.Grades)
                .ToListAsync();


            if (teachers == null)
                return NotFound();

            var dtoList = teachers.Select(t => new Teacherdto
            {
                Id = t.Id,
                UserId = t.UserId,
                Teacher_Name = t.Teacher_Name,
                Email = t.Email,
                T_courses = t.T_courses.Select(c => new Courses
                {
                    Id = c.Id,
                    Course_Name = c.Course_Name,
                    Description = c.Description,
                    Course_Hours = c.Course_Hours,
                    isDone = c.isDone,
                    isApproved = c.isApproved,
                    Students = c.Students.Select(s => new Students
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Email = s.Email,
                        Grades = s.Grades.Select(g => new Grades
                        {
                            CourseId = g.CourseId,
                            first = g.first,
                            second = g.second,
                            final = g.final,
                            total = g.total
                        }).ToList(),
                    }).ToList(),
                }).ToList()
            }).ToList();

            return Ok(dtoList);
        }
        // PUT: api/Teachers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTeachers(int id, string Phone, string Email, int Salary)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if(teacher == null)
            {
                return NotFound();
            }

            //_context.Entry(teachers).State = EntityState.Modified;
            teacher.Phone = Phone;
            teacher.Email = Email;
            teacher.Salary = Salary;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeachersExists(id))
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

        [HttpPut("{teacherId}/Courses")]
        public async Task<IActionResult> PutTeachersCourses(int teacherId, int courseId)
        {
            var teacher = await _context.Teachers.FindAsync(teacherId);
            if (teacher == null)
            {
                return NotFound();
            }

            //_context.Entry(teachers).State = EntityState.Modified;
            foreach (var course in teacher.T_courses)
            {
                if (course.Id == teacherId) 
                { 
                    teacher.T_courses.Remove(course);
                }
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeachersExists(teacherId))
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
        // POST: api/Teachers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Teachers>> PostTeachers(Teachers teachers)
        {
            _context.Teachers.Add(teachers);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTeachers", new { id = teachers.Id }, teachers);
        }

        // DELETE: api/Teachers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeachers(int id)
        {
            var teachers = await _context.Teachers.FindAsync(id);
            if (teachers == null)
            {
                return NotFound();
            }

            _context.Teachers.Remove(teachers);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TeachersExists(int id)
        {
            return _context.Teachers.Any(e => e.Id == id);
        }
    }
}
