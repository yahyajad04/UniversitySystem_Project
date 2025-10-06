using API_Consumer_University_test1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

namespace API_Consumer_University_test1.Controllers
{
    [Authorize]
    public class TeachersController : Controller
    {
        Uri address_GetTeachers = new Uri("http://localhost:5134/api/Teachers");
        private readonly HttpClient _httpClient;
        private readonly UserManager<IdentityUser> _userManager;

        public TeachersController(UserManager<IdentityUser> userManager)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = address_GetTeachers;
            _userManager = userManager;
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            List<Teachers> teacher = new List<Teachers>();
            HttpResponseMessage response = await _httpClient.GetAsync(address_GetTeachers);
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                teacher = JsonConvert.DeserializeObject<List<Teachers>>(data);
            }
            return View(teacher);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ShowProfileAdmin(int id)
        {
            var teacher = new Teachers();
            HttpResponseMessage response = await _httpClient.GetAsync($"{address_GetTeachers}/{id}");
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                teacher = JsonConvert.DeserializeObject<Teachers>(data);
            }

            return View(teacher);
        }

        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> ShowProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Failed to get the User Logged In";
                return RedirectToAction("Index", "Home");
            }
            var UserId = user.Id;

            List<Teachers> teachers = new List<Teachers>();
            HttpResponseMessage response = await _httpClient.GetAsync(address_GetTeachers);
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                teachers = JsonConvert.DeserializeObject<List<Teachers>>(data);
            }
            foreach (var teacher in teachers)
            {
                if (teacher.UserId == UserId)
                {
                    return View(teacher);
                }
            }
            TempData["ErrorMessage"] = "Failed to get the teacher Profile";
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTeacher()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTeacher(Teachers teacher)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Creation Failed: Model is invalid";
                return RedirectToAction("Index");
            }

            var existingUser = await _userManager.FindByEmailAsync(teacher.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "This email is already used. Please choose another one.");
                return View(teacher);
            }

            var user = new IdentityUser
            {
                UserName = teacher.Email,
                Email = teacher.Email
            };
            var result = await _userManager.CreateAsync(user, teacher.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
                return View(teacher);
            }
            await _userManager.AddToRoleAsync(user, "Teacher");

            teacher.UserId = user.Id;

            //Here we start with preparing the data we want to send to POST method in API
            //the studentForApi variable is used to sepecify what we want to insert into the post header
            var teacherForApi = new
            {
                teacher.Teacher_Name,
                teacher.Email,
                teacher.Phone,
                teacher.PHD,
                teacher.PHD_University,
                teacher.Salary,
                teacher.UserId
            };
            var json = JsonConvert.SerializeObject(teacherForApi);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("", content);

            if (!response.IsSuccessStatusCode)
            {
                TempData["error"] = "Creation Failed: response is not successful";
                await _userManager.DeleteAsync(user);
                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, error);
                return View(teacher);
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTeacher(int teacherId, string Email)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Failed to delete; Model Failed.";
                return RedirectToAction("Index");
            }

            var existingUser = await _userManager.FindByEmailAsync(Email);
            if (existingUser == null)
            {
                TempData["ErrorMessage"] = "Failed to delete : email not found.";
                ModelState.AddModelError("Email", "This email is not used. Please choose another one.");
                return RedirectToAction("Index");
            }
            List<Teachers> teachers = new List<Teachers>();
            HttpResponseMessage response = await _httpClient.GetAsync(address_GetTeachers);
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                teachers = JsonConvert.DeserializeObject<List<Teachers>>(data);
            }
            
            foreach (var teacher1 in teachers)
                {
                Debug.WriteLine(existingUser.Id + "this is what is sored for the teacher: " + teacher1.UserId);
                if (teacher1.UserId == existingUser.Id && teacher1.Id == teacherId)
                    {
                    
                        await _userManager.DeleteAsync(existingUser);
                        var deleteResponse = await _httpClient.DeleteAsync($"{address_GetTeachers}/{teacher1.Id}");
                        if (!deleteResponse.IsSuccessStatusCode)
                        {
                            TempData["ErrorMessage"] = "Failed to delete teacher from database.";
                            return RedirectToAction("Index");
                        }
                    }
                }

            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> DeleteConfirmed(int teacherId)
        {
            List<Teachers> teachers = new List<Teachers>();
            HttpResponseMessage response = _httpClient.GetAsync(address_GetTeachers).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                teachers = JsonConvert.DeserializeObject<List<Teachers>>(data);
            }
            foreach (var teacher in teachers)
            {
                if (teacher.Id == teacherId)
                {
                    return View(teacher);
                }
            }
            TempData["ErrorMessage"] = "Failed to Find the Student";
            return RedirectToAction("Index");
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditTeacher(int id)
        {
            var teacher = await _httpClient.GetFromJsonAsync<Teachers>($"http://localhost:5134/api/Teachers/{id}");

            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditTeacher(int id, string phone, string email,int salary)
        {
            var teacher = await _httpClient.GetFromJsonAsync<Teachers>($"http://localhost:5134/api/Teachers/{id}");

            if (teacher == null)
            {
                TempData["ErrorMessage"] = "Student couldnt be found";
                return RedirectToAction("Index");
            }
            var EmailUser = teacher.Email;
            var user = await _userManager.FindByEmailAsync(EmailUser);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User couldnt be found";
                return RedirectToAction("Index");
            }
            var putUrl = $"http://localhost:5134/api/Teachers/{id}?Phone={phone}&Email={email}&Salary={salary}";
            var putResponse = await _httpClient.PutAsync(putUrl, null);
            if (!putResponse.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Failed to Edit values.";
            }
            user.Email = email;
            user.NormalizedEmail = email.ToUpper();
            user.UserName = email;
            user.NormalizedUserName = email.ToUpper();
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = "Failed to update Identity user.";
                return RedirectToAction("ShowProfile");
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> RegisteredCourses()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Failed to get the User Logged In";
                return RedirectToAction("Index", "Home");
            }
            List<Teachers> teachers = new List<Teachers>();
            HttpResponseMessage response = await _httpClient.GetAsync($"{address_GetTeachers}/Dto");
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                teachers = JsonConvert.DeserializeObject<List<Teachers>>(data);
            }
            foreach (var teacher in teachers)
            {
                if (teacher.UserId == user.Id) {
                    //teacher.T_courses = teacher.T_courses
                    //    .Where(c => c.isDone == 0 || c.isDone==null) // only unfinished courses
                    //    .ToList();
                    return View(teacher);
                } 
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> RemoveCourse(int teacherId, int courseId)
        {
            var teacher = new Teachers();
            HttpResponseMessage response = await _httpClient.GetAsync($"{address_GetTeachers}/{teacherId}");
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                teacher = JsonConvert.DeserializeObject<Teachers>(data);
            }
            var putUrl = $"http://localhost:5134/api/Teachers/{teacherId}/Courses?courseId={courseId}";
            var putResponse = await _httpClient.PutAsync(putUrl, null);
            if (!putResponse.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Failed to Edit values.";
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> StudentsinCourse(int courseId)
        {
            Courses course = new Courses();
            HttpResponseMessage response = await _httpClient.GetAsync($"http://localhost:5134/api/Courses/{courseId}");
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                course = JsonConvert.DeserializeObject<Courses>(data);
            }
            return View(course);
        }
    }
}
