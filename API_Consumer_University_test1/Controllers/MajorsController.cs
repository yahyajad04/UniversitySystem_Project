using API_Consumer_University_test1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace API_Consumer_University_test1.Controllers
{
    public class MajorsController : Controller
    {
        private static string _token;
        Uri address_Majors = new Uri("http://localhost:5134/api/Majors");
        private readonly HttpClient _httpClient;
        public MajorsController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = address_Majors;
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
        public async Task<IActionResult> Index()
        {
            await LoginAsync();
            List<Majors> majors = new List<Majors>();
            HttpResponseMessage response = _httpClient.GetAsync(address_Majors).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                majors = JsonConvert.DeserializeObject<List<Majors>>(data);
            }
            return View(majors);
        }
        public IActionResult CreateMajor()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateMajor(Majors major)
        {
            await LoginAsync();
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Creation Failed";
                return View(major);
            }
            List<Majors> majors = new List<Majors>();
            HttpResponseMessage response = _httpClient.GetAsync(address_Majors).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                majors = JsonConvert.DeserializeObject<List<Majors>>(data);
            }
            if (majors.Any(m => m.Name == major.Name)) {
                TempData["error"] = "Creation Failed: Major already exists";
                return RedirectToAction("Index");
            }
            var majorforAPI = new
            {
                major.Name,
                major.major_cost_hour,
                major.major_hours
            };
            var json = JsonConvert.SerializeObject(majorforAPI);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response2 = await _httpClient.PostAsync("", content);

            if (!response2.IsSuccessStatusCode)
            {
                TempData["error"] = "Creation Failed";
                return View(major);
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteMajor(int majorId)
        {
            await LoginAsync();
            if (!ModelState.IsValid)
                return RedirectToAction("Index");


            List<Majors> majors = new List<Majors>();
            HttpResponseMessage response = _httpClient.GetAsync(address_Majors).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                majors = JsonConvert.DeserializeObject<List<Majors>>(data);
            }

            foreach (var major in majors)
            {
                if (major.Id == majorId)
                {
                    var deleteResponse = await _httpClient.DeleteAsync($"{address_Majors}/{major.Id}");
                    if (!deleteResponse.IsSuccessStatusCode)
                    {
                        TempData["ErrorMessage"] = "Failed to delete Major from database.";
                        return RedirectToAction("Index");
                    }
                }
            }

            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> DeleteConfirmed(int majorId)
        {
            await LoginAsync();
            List<Majors> majors = new List<Majors>();
            HttpResponseMessage response = _httpClient.GetAsync(address_Majors).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                majors = JsonConvert.DeserializeObject<List<Majors>>(data);
            }
            foreach (var major in majors)
            {
                if (major.Id == majorId)
                {
                    TempData["AlertMessage"] = "By deleting the Major you have to make sure to delete the Students that have the same Major";
                    return View(major);
                }
            }
            TempData["ErrorMessage"] = "Failed to Find the Major";
            return RedirectToAction("Index");
        }
    }
}
