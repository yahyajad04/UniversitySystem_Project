using API_Consumer_University_test1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace API_Consumer_University_test1.Controllers
{
    public class CoursesController : Controller
    {
        Uri address_Courses = new Uri("https://university-api-dag5drgrauh3fzbp.uaenorth-01.azurewebsites.net/api/Courses");
        private readonly HttpClient _httpClient;
        public CoursesController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = address_Courses;
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
        public IActionResult CreateCourse()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateCourse(Courses course)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Creation Failed: Model is invalid";
                return View(course);
            }
            List<Courses> courses = new List<Courses>();
            HttpResponseMessage response = _httpClient.GetAsync(address_Courses).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                courses = JsonConvert.DeserializeObject<List<Courses>>(data);
            }
            if (courses.Any(c => c.Course_Name == course.Course_Name))
            {
                TempData["error"] = "Creation Failed: Course already exists";
                return RedirectToAction("Index", "Students");
            }
            var courseforAPI = new
            {
                course.Course_Name,
                course.Teacher_Name,
                course.Description,
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
                    var deleteResponse = await _httpClient.DeleteAsync($"{address_Courses}/{course.Id}");
                    if (!deleteResponse.IsSuccessStatusCode)
                    {
                        TempData["ErrorMessage"] = "Failed to delete course from database.";
                        return RedirectToAction("Index");
                    }
                }
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
                    TempData["AlertMessage"] = "By Deleting this course you have to edit the hours/term and the reciept for the students that enrolled the course.";
                    return View(course);
                }
            }
            TempData["ErrorMessage"] = "Failed to Find the Course";
            return RedirectToAction("Index");
        }
    }
}
