using API_Consumer_University_test1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Newtonsoft.Json;
using System.Diagnostics;

namespace API_Consumer_University_test1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        Uri address_GetStudents = new Uri("http://localhost:5134/api/Students");
        private readonly HttpClient _httpClient;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = address_GetStudents;
        }

        public async Task<IActionResult> Index()
        {
            List<Students> student = new List<Students>();
            HttpResponseMessage response = await _httpClient.GetAsync(address_GetStudents);
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                student = JsonConvert.DeserializeObject<List<Students>>(data);
            }
            List<Courses> courses = new List<Courses>();
            HttpResponseMessage response1 = await _httpClient.GetAsync("http://localhost:5134/api/Courses");
            if (response1.IsSuccessStatusCode)
            {
                string data = response1.Content.ReadAsStringAsync().Result;
                courses = JsonConvert.DeserializeObject<List<Courses>>(data);
            }
            List<Majors> majors = new List<Majors>();
            HttpResponseMessage response2 = await _httpClient.GetAsync("http://localhost:5134/api/Majors");
            if (response1.IsSuccessStatusCode)
                if (response2.IsSuccessStatusCode)
            {
                string data = response2.Content.ReadAsStringAsync().Result;
                majors = JsonConvert.DeserializeObject<List<Majors>>(data);
            }
            var totalStudents = student.Count;
            var totalMajors = majors.Count;
            var totalCourses = courses.Count;

            ViewBag.TotalStudents = totalStudents;
            ViewBag.TotalMajors = totalMajors;
            ViewBag.TotalCourses = totalCourses;
            ViewBag.courses = courses;
            ViewBag.students = student;
            ViewBag.majors = majors;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ApproveCourse()
        {
            List<Courses> courses = new List<Courses>();
            HttpResponseMessage response1 = await _httpClient.GetAsync("http://localhost:5134/api/Courses");
            if (response1.IsSuccessStatusCode)
            {
                string data = response1.Content.ReadAsStringAsync().Result;
                courses = JsonConvert.DeserializeObject<List<Courses>>(data);
            }
            return View(courses);
        }
        [HttpPost]
        public async Task<IActionResult> ApproveCourse(int id,int isApproved)
        {
            Debug.WriteLine($"++ Course Id == {id}");
            var putUrl = $"http://localhost:5134/api/Courses/{id}/isApproved?isApproved={isApproved}";
            var putResponse = await _httpClient.PutAsync(putUrl, null);
            if (!putResponse.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = $"Failed to Edit values. Status: {putResponse.StatusCode}";
                return RedirectToAction("Index","Courses");
            }
            return RedirectToAction("Index", "Courses");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
