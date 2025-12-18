using API_Consumer_University_test1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using static System.Net.WebRequestMethods;

namespace API_Consumer_University_test1.Controllers
{
    public class CoursesController : Controller
    {
        private static string _token;
        Uri address_Courses = new Uri("http://localhost:5134/api/Courses");
        private readonly HttpClient _httpClient;
        private readonly UserManager<IdentityUser> _userManager;
        public CoursesController(UserManager<IdentityUser> userManager)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = address_Courses;
            _userManager = userManager;
        }

        private async Task LoginAsync()
        {
            var loginData = new { userName = "admin", password = "Password" };
            var content = new StringContent(JsonConvert.SerializeObject(loginData), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"http://localhost:5134/api/APILogin", content);

            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception("Login failed: " + result);

            dynamic obj = JsonConvert.DeserializeObject(result);
            _token = obj.token;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        }
        public async Task<IActionResult> Index(string sortField = "Course_Name", string sortOrder = "asc")
        {
            await LoginAsync();
            List<Courses> courses = new List<Courses>();
            HttpResponseMessage response = _httpClient.GetAsync(address_Courses).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                courses = JsonConvert.DeserializeObject<List<Courses>>(data);
            }
            switch (sortField)
            {
                case "Course_Name":
                    courses = sortOrder == "desc"
                        ? courses.OrderByDescending(c => c.Course_Name).ToList()
                        : courses.OrderBy(c => c.Course_Name).ToList();
                    break;

                case "Id":
                    courses = sortOrder == "desc"
                        ? courses.OrderByDescending(c => c.Id).ToList()
                        : courses.OrderBy(c => c.Id).ToList();
                    break;
            }
            return View(courses);
        }
        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> CreateCourse()
        {
            await LoginAsync();
            List<Majors> Majors = new List<Majors>();
            HttpResponseMessage response2Majors = await _httpClient.GetAsync("http://localhost:5134/api/Majors");
            if (response2Majors.IsSuccessStatusCode)
            {
                string data4 = await response2Majors.Content.ReadAsStringAsync();
                Majors = JsonConvert.DeserializeObject<List<Majors>>(data4);
            }
            else
            {
                Console.WriteLine("FAILED MAJORS API");
            }
            ViewBag.Majors = Majors;
            return View();
        }
        [HttpPost]
        [Authorize(Roles ="Teacher")]
        public async Task<IActionResult> CreateCourse(Courses course, List<int> majors)
        {
            await LoginAsync();
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
            List<Majors> Majors = new List<Majors>();
            HttpResponseMessage response2Majors = await _httpClient.GetAsync("http://localhost:5134/api/Majors");
            if (response2Majors.IsSuccessStatusCode)
            {
                string data4 = await response2Majors.Content.ReadAsStringAsync();
                Majors = JsonConvert.DeserializeObject<List<Majors>>(data4);
            }
            else
            {
                Console.WriteLine("FAILED MAJORS API");
            }
            ViewBag.Majors = Majors;
           
            var courseforAPI = new
            {
                course.Course_Name,
                course.Description,
                TeacherId = teacherId,
                course.Course_Hours,
                MajorsId = majors,
            };
            var json = JsonConvert.SerializeObject(courseforAPI);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response2 = await _httpClient.PostAsync("", content);

            if (!response2.IsSuccessStatusCode)
            {
                TempData["error"] = "Creation Failed: API Failed";
                return View(course);
            }

            return RedirectToAction("RegisteredCourses", "Teachers");
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCourse(int courseId)
        {
            await LoginAsync();
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
            await LoginAsync();
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
            await LoginAsync();
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
            await LoginAsync();
            Courses course = new Courses();
            HttpResponseMessage response = _httpClient.GetAsync($"{address_Courses}/{courseId}").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                course = JsonConvert.DeserializeObject<Courses>(data);
            }
            return View(course);
        }
        [HttpGet]
        public async Task<IActionResult> EditCourse(int courseId)
        {
            await LoginAsync();
            Courses course = new Courses();
            HttpResponseMessage response = await _httpClient.GetAsync($"{address_Courses}/{courseId}");
            if (response.IsSuccessStatusCode)
            {
                string data4 = await response.Content.ReadAsStringAsync();
                course = JsonConvert.DeserializeObject<Courses>(data4);
            }
            return View(course);
        }
        [HttpPost]
        public async Task<IActionResult> EditCourse(int courseId, string Course_Name, string Description)
        {
            await LoginAsync();
            var putUrl = $"http://localhost:5134/api/Courses/{courseId}?CName={Course_Name}&Description={Description}";
            var putResponse = await _httpClient.PutAsync(putUrl, null);
            if (!putResponse.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Failed to Edit values.";
            }
            Debug.WriteLine($"++ Course Id == {courseId}");
            Debug.WriteLine($"++ Course Name == {Course_Name}");
            return RedirectToAction("Index");
        }
    }
}
