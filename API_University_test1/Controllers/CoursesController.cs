using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_University_test1.Data;
using API_University_test1.Models;
using Microsoft.AspNetCore.Authorization;

namespace API_University_test1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CoursesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CoursesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Courses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Coursedto>>> GetCourses()
        {
            var courses = await _context.Courses
                .Include(t => t.Teacher)
                .Include(m => m.Majors)
                .Include(s => s.Students)
                .ThenInclude(s => s.Major)
                .ToListAsync();
            foreach(var course in courses)
            {
                if(course.isApproved == null)
                {
                    course.isApproved = 0;
                }
            }
            List<Coursedto> dto_list = new List<Coursedto>();
            foreach (var course in courses)
            {
                dto_list.Add(new Coursedto
                {
                    Id = course.Id,
                    Course_Name = course.Course_Name,
                    TeacherId = course.TeacherId,
                    Teacher = course.Teacher,
                    Course_Hours = course.Course_Hours,
                    isApproved = course.isApproved,
                    isDone = course.isDone,
                    Majors = course.Majors,
                    Description = course.Description,
                    Students = course.Students.Select(c => new Studentdto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Email = c.Email,
                        reciept = c.reciept,
                        hours_term = c.hours_term,
                        Major = c.Major,
                        Grades = c.Grades.Select(g => new Grades
                        {
                            Id = g.Id,
                            CourseId = g.CourseId,
                            StudentId = g.StudentId,
                            first = g.first,
                            second = g.second,
                            final = g.final
                        }).ToList()
                    }).ToList()
                });
            }

            return Ok(dto_list);

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Courses>> GetCourse(int id)
        {
            {
                var course = await _context.Courses
                    .Include(t => t.Teacher)
                    .Include(m => m.Majors)
                    .Include(s => s.Students)
                    .ThenInclude(g => g.Grades)
                    .Include(s => s.Students)
                    .ThenInclude(s => s.Major)
                    .FirstOrDefaultAsync(s => s.Id == id);
                
                    if (course.isApproved == null)
                    {
                        course.isApproved = 0;
                    }
                
                if (course == null) return NotFound();

                var dto = new Coursedto
                {
                    Id = course.Id,
                    Course_Name = course.Course_Name,
                    TeacherId = course.TeacherId,
                    Teacher = course.Teacher,
                    Course_Hours = course.Course_Hours,
                    isApproved = course.isApproved,
                    isDone = course.isDone,
                    Majors = course.Majors,
                    Description = course.Description,
                    Students = course.Students.Select(c => new Studentdto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Email = c.Email,
                        reciept = c.reciept,
                        hours_term = c.hours_term,
                        Major = c.Major,
                        Grades = c.Grades.Select(g => new Grades
                        {
                            Id = g.Id,
                            CourseId = g.CourseId,
                            StudentId = g.StudentId,
                            first = g.first,
                            second = g.second,
                            final = g.final
                        }).ToList()
                    }).ToList()
                };

                return Ok(dto);
            }
        }


        // PUT: api/Courses/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCourses(int id, string CName, string Description)
        {
            var course = _context.Courses.FirstOrDefault(c => c.Id == id);
            if (course == null)
            {
                return BadRequest();
            }

            course.Course_Name = CName;
            course.Description = Description;
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
        [HttpPut("{id}/isApproved")]
        public async Task<IActionResult> PutCoursesisApproved(int id, int isApproved)
        {
            var course = _context.Courses.FirstOrDefault(c => c.Id == id);

            if (course == null) 
            {
                return NotFound();
            }

            course.isApproved = isApproved;
            //_context.Entry(courses).State = EntityState.Modified;

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
        [HttpPut("{id}/isDone")]
        public async Task<IActionResult> PutCoursesisDone(int id)
        {
            var course = _context.Courses
            .Include(c => c.Students)
            .ThenInclude(s => s.Major)
            .Include(t => t.Teacher)
            .FirstOrDefault(c => c.Id == id);


            if (course == null)
            {
                return NotFound();
            }

            if (course.isDone == 0 || course.isDone == null) {
                course.isDone = 1;
                var grades = await _context.Grades
                            .Where(g => g.CourseId == course.Id)
                            .ToListAsync();
                foreach (var grade in grades)
                {
                    if (grade.total >= 85) { grade.letter_grade = "A"; }
                    else if (grade.total >= 77) { grade.letter_grade = "A-"; }
                    else if (grade.total >= 72) { grade.letter_grade = "B+"; }
                    else if (grade.total >= 65) { grade.letter_grade = "B"; }
                    else if (grade.total >= 60) { grade.letter_grade = "B-"; }
                    else if (grade.total >= 56) { grade.letter_grade = "C+"; }
                    else if (grade.total >= 52) { grade.letter_grade = "C"; }
                    else if (grade.total >= 48) { grade.letter_grade = "C-"; }
                    else if (grade.total >= 45) { grade.letter_grade = "D+"; }
                    else if (grade.total >= 41) { grade.letter_grade = "D"; }
                    else if (grade.total >= 36) { grade.letter_grade = "D-"; }
                    else if (grade.total < 36) { grade.letter_grade = "F"; }
                }
                course.Teacher.T_courses.Remove(course);
                foreach (var student in course.Students)
                {
                    student.hours_term -= course.Course_Hours;
                    student.Total_Hours += course.Course_Hours;
                    student.reciept -= (course.Course_Hours * student.Major.major_cost_hour);
                }
            }

            //_context.Entry(courses).State = EntityState.Modified;

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
            courses.isDone = 0;
            courses.isApproved = 0;
            if (courses.MajorsId != null)
            {
                foreach (var mid in courses.MajorsId)
                {
                    courses.Majors.Add(await _context.Majors.FirstOrDefaultAsync(m => m.Id == mid));
                }
            }
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
