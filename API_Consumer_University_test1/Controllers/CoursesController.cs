using API_Consumer_University_test1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

namespace API_Consumer_University_test1.Controllers
{
    public class CoursesController : Controller
    {
        Uri address_Courses = new Uri("http://localhost:5134/api/Courses");
        private readonly HttpClient _httpClient;
        private readonly UserManager<IdentityUser> _userManager;
        public CoursesController(UserManager<IdentityUser> userManager)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = address_Courses;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            List<Courses> courses = new List<Courses>();
            HttpResponseMessage response = _httpClient.GetAsync(address_Courses).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                courses = JsonConvert.DeserializeObject<List<Courses>>(data);
            }
            return View(courses);
        }
        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public IActionResult CreateCourse()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles ="Teacher")]
        public async Task<IActionResult> CreateCourse(Courses course/*, int teacherId*/)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Creation Failed: Model is invalid";
                return View(course);
            }
            List<Teachers> teachers = new List<Teachers>();
            HttpResponseMessage response2Teachers = _httpClient.GetAsync("http://localhost:5134/api/Teachers").Result;
            if (response2Teachers.IsSuccessStatusCode)
            {
                string data = response2Teachers.Content.ReadAsStringAsync().Result;
                teachers = JsonConvert.DeserializeObject<List<Teachers>>(data);
            }
            var UserId = _userManager.GetUserId(User);
            var teacherId = 0;
            foreach(var teacher in teachers)
            {
                if (teacher.UserId == UserId) 
                {
                    teacherId = teacher.Id;
                }
            }
            var courseforAPI = new
            {
                course.Course_Name,
                course.Description,
                TeacherId = teacherId,
                course.Course_Hours,
                
            };
            var json = JsonConvert.SerializeObject(courseforAPI);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response2 = await _httpClient.PostAsync("", content);

            if (!response2.IsSuccessStatusCode)
            {
                TempData["error"] = "Creation Failed: API Failed";
                return View(course);
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCourse(int courseId)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index");

           
            var course = new Courses();
            HttpResponseMessage response = await _httpClient.GetAsync($"http://localhost:5134/api/Courses/{courseId}");
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                course = JsonConvert.DeserializeObject<Courses>(data);
            }
            if (!course.Students.IsNullOrEmpty())
            {
                foreach (var student in course.Students)
                {
                    var new_reciept = student.reciept - course.Course_Hours * student.Major.major_cost_hour;
                    var new_hours_term = student.hours_term - course.Course_Hours;
                    var putUrl = $"http://localhost:5134/api/Students/{student.Id}/reciept?hours_term={new_hours_term}&reciept={new_reciept}";
                    var putResponse = await _httpClient.PutAsync(putUrl, null);
                    if (!putResponse.IsSuccessStatusCode)
                    {
                        TempData["ErrorMessage"] = "Failed to Edit reciept values for students.";
                        return RedirectToAction("Index");
                    }
                }
            }
            var deleteResponse = await _httpClient.DeleteAsync($"http://localhost:5134/api/Courses/{courseId}");
                    if (!deleteResponse.IsSuccessStatusCode)
                    {
                        TempData["ErrorMessage"] = "Failed to delete course from database.";
                        return RedirectToAction("Index");
                    }
                    

            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> DeleteConfirmed(int courseId)
        {
            List<Courses> courses = new List<Courses>();
            HttpResponseMessage response = _httpClient.GetAsync(address_Courses).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                courses = JsonConvert.DeserializeObject<List<Courses>>(data);
            }
            foreach (var course in courses)
            {
                if (course.Id == courseId)
                {
                    return View(course);
                }
            }
            TempData["ErrorMessage"] = "Failed to Find the Course";
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> EndCourse(int courseId)
        {
            var putUrl = $"http://localhost:5134/api/Courses/{courseId}/isDone";
            var putResponse = await _httpClient.PutAsync(putUrl, null);
            if (!putResponse.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Failed to edit the isDone field.";
            }
            return RedirectToAction("RegisteredCourses", "Teachers");
        }
        public async Task<IActionResult> EndCourseConfirm(int courseId)
        {
            Courses course = new Courses();
            HttpResponseMessage response = _httpClient.GetAsync($"{address_Courses}/{courseId}").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                course = JsonConvert.DeserializeObject<Courses>(data);
            }
            return View(course);
        }
    }
}
