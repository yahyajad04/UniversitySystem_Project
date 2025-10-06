using API_Consumer_University_test1.Data;
using API_Consumer_University_test1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace API_Consumer_University_test1.Controllers
{
    [Authorize]
    public class StudentsController : Controller
    {
        Uri address_GetStudents = new Uri("http://localhost:5134/api/Students");
        private readonly HttpClient _httpClient;
        private readonly UserManager<IdentityUser> _userManager;


        public StudentsController(UserManager<IdentityUser> userManager)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = address_GetStudents;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Students> student = new List<Students>();
            HttpResponseMessage response = await _httpClient.GetAsync(address_GetStudents);
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                student = JsonConvert.DeserializeObject<List<Students>>(data);
            }
            return View(student);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ShowProfileAdmin(int id)
        {
            List<Students> students = new List<Students>();
            HttpResponseMessage response = await _httpClient.GetAsync(address_GetStudents);
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                students = JsonConvert.DeserializeObject<List<Students>>(data);
            }
            foreach (var student in students)
            {
                if (student.Id == id)
                {
                    return View(student);
                }
            }
            TempData["ErrorMessage"] = "Failed to get the student Profile";
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> ShowProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Failed to get the User Logged In";
                return RedirectToAction("Index", "Home");
            }
            var UserId = user.Id;

            List<Students> students = new List<Students>();
            HttpResponseMessage response = await _httpClient.GetAsync(address_GetStudents);
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                students = JsonConvert.DeserializeObject<List<Students>>(data);
            }
            foreach (var student in students)
            {
                if (student.UserId == UserId)
                {
                    return View(student);
                }
            }
            TempData["ErrorMessage"] = "Failed to get the student Profile";
            return RedirectToAction("Index", "Home");
        }
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Enroll(string sortField = "Course_Name", string sortOrder = "asc")
        {
            List<Courses> course = new List<Courses>();
            HttpResponseMessage response = await _httpClient.GetAsync("http://localhost:5134/api/Courses");
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                course = JsonConvert.DeserializeObject<List<Courses>>(data);
            }
            if (course != null)
            {

                foreach (var course1 in course)
                {
                    if (course1.isDone == null)
                    {
                        course1.isDone = 0;
                    }
                }
                course.RemoveAll(c => c.isDone > 0);
                switch (sortField)
                {
                    case "Course_Name":
                        course = sortOrder == "desc"
                            ? course.OrderByDescending(c => c.Course_Name).ToList()
                            : course.OrderBy(c => c.Course_Name).ToList();
                        break;

                    case "Id":
                        course = sortOrder == "desc"
                            ? course.OrderByDescending(c => c.Id).ToList()
                            : course.OrderBy(c => c.Id).ToList();
                        break;

                    case "Teacher_Name":
                        course = sortOrder == "desc"
                            ? course.OrderByDescending(c => c.Teacher.Teacher_Name).ToList()
                            : course.OrderBy(c => c.Teacher.Teacher_Name).ToList();
                        break;
                }
            }
            return View(course);
        }
        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> EnrollCourse(int courseId)
        {
            List<Courses> courses = new List<Courses>();
            HttpResponseMessage response2 = await _httpClient.GetAsync("http://localhost:5134/api/Courses");
            if (response2.IsSuccessStatusCode)
            {
                string data2 = await response2.Content.ReadAsStringAsync();
                courses = JsonConvert.DeserializeObject<List<Courses>>(data2);
            }
            string userId = _userManager.GetUserId(User);
            HttpResponseMessage response1 = await _httpClient.GetAsync($"http://localhost:5134/api/Students/byUser/{userId}");
            if (!response1.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Failed to open page: not logged in with a valid student account";
                return RedirectToAction("Index", "Home");
            }

            string data = await response1.Content.ReadAsStringAsync();
            var student = JsonConvert.DeserializeObject<Students>(data);
            foreach (var course in courses)
            {
                if (course.Id == courseId)
                {
                    student.hours_term += course.Course_Hours;
                    if (student.hours_term > 18)
                    {
                        student.hours_term -= course.Course_Hours;
                        TempData["ErrorMessage"] = "Failed to Enroll: You exceeded the maximum hours in term";
                        return RedirectToAction("Enroll");

                    }
                }
            }

            student.reciept = student.hours_term * student.Major.major_cost_hour;
            var StudentId = student.Id;
            var values = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("courseId", courseId.ToString())
                };
            var content = new FormUrlEncodedContent(values);
            var response = await _httpClient.PostAsync($"http://localhost:5134/api/Students/{StudentId}/courses", content);
            if (response.IsSuccessStatusCode)
            {
                var putUrl = $"http://localhost:5134/api/Students/{StudentId}/reciept?hours_term={student.hours_term}&reciept={student.reciept}";
                var putResponse = await _httpClient.PutAsync(putUrl, null);
                if (!putResponse.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Enrolled but failed to update receipt/hours.";
                }
                return RedirectToAction("RegesteredCourses");
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to Enroll: The Student is already Enrolled in this Course";
                return RedirectToAction("RegesteredCourses");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Unroll(int courseId)
        {
            List<Courses> courses = new List<Courses>();
            HttpResponseMessage response2 = _httpClient.GetAsync("http://localhost:5134/api/Courses").Result;
            if (response2.IsSuccessStatusCode)
            {
                string data2 = response2.Content.ReadAsStringAsync().Result;
                courses = JsonConvert.DeserializeObject<List<Courses>>(data2);
            }

            string userId = _userManager.GetUserId(User);
            HttpResponseMessage response1 = await _httpClient.GetAsync($"http://localhost:5134/api/Students/byUser/{userId}");
            if (!response1.IsSuccessStatusCode)
            {
                return NotFound();
            }

            string data = await response1.Content.ReadAsStringAsync();
            var student = JsonConvert.DeserializeObject<Students>(data);

            foreach (var course in courses)
            {
                if (course.Id == courseId)
                    student.hours_term -= course.Course_Hours;
            }

            student.reciept = student.hours_term * student.Major.major_cost_hour;

            var StudentId = student.Id;
            var values = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("courseId", courseId.ToString())
                };
            var content = new FormUrlEncodedContent(values);
            var response = await _httpClient.PostAsync($"http://localhost:5134/api/Students/{StudentId}/courses/unroll", content);
            if (response.IsSuccessStatusCode)
            {
                var putUrl = $"http://localhost:5134/api/Students/{StudentId}/reciept?hours_term={student.hours_term}&reciept={student.reciept}";
                var putResponse = await _httpClient.PutAsync(putUrl, null);
                if (!putResponse.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Enrolled but failed to update receipt/hours.";
                }
                return RedirectToAction("RegesteredCourses");
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to Unroll: The Student is not Enrolled in this Course";
                return RedirectToAction("RegesteredCourses");
            }
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateStudent()
        {
            List<Majors> majors = new List<Majors>();
            HttpResponseMessage response = await _httpClient.GetAsync("http://localhost:5134/api/Majors");
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                majors = JsonConvert.DeserializeObject<List<Majors>>(data);
            }

            ViewBag.Majors = majors;
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateStudent(Students student)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Creation Failed";
                return RedirectToAction("Index");
            }
            //This is for getting the Majors List so we can make the admin choose the Major from the Majors we have in the system
            List<Majors> majors = new List<Majors>();
            HttpResponseMessage response2 = _httpClient.GetAsync("http://localhost:5134/api/Majors").Result;
            if (response2.IsSuccessStatusCode)
            {
                string data = response2.Content.ReadAsStringAsync().Result;
                majors = JsonConvert.DeserializeObject<List<Majors>>(data);
            }
            ViewBag.Majors = majors;
            var existingUser = await _userManager.FindByEmailAsync(student.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "This email is already used. Please choose another one.");
                ViewBag.Majors = majors;
                return View(student);
            }

            var user = new IdentityUser
            {
                UserName = student.Email,
                Email = student.Email
            };
            var result = await _userManager.CreateAsync(user, student.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
                ViewBag.Majors = majors;
                return View(student);
            }
            await _userManager.AddToRoleAsync(user, "Student");

            student.UserId = user.Id;

            //Here we start with preparing the data we want to send to POST method in API
            //the studentForApi variable is used to sepecify what we want to insert into the post header
            student.hours_term = 0;
            student.reciept = 0;
            var studentForApi = new
            {
                student.Name,
                student.Email,
                student.Phone,
                student.MajorsId,
                student.hours_term,
                student.reciept,
                student.UserId
            };
            var json = JsonConvert.SerializeObject(studentForApi);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("", content);

            if (!response.IsSuccessStatusCode)
            {
                await _userManager.DeleteAsync(user);
                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, error);
                ViewBag.Majors = majors;
                return View(student);
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteStudent(int studentId, string Email)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index");

            var existingUser = await _userManager.FindByEmailAsync(Email);
            if (existingUser == null)
            {
                ModelState.AddModelError("Email", "This email is not used. Please choose another one.");
                return RedirectToAction("Index");
            }

            List<Students> students = new List<Students>();
            HttpResponseMessage response = _httpClient.GetAsync(address_GetStudents).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                students = JsonConvert.DeserializeObject<List<Students>>(data);
            }

            foreach (var student1 in students)
            {
                if (student1.UserId == existingUser.Id && student1.Id == studentId)
                {
                    await _userManager.DeleteAsync(existingUser);
                    var deleteResponse = await _httpClient.DeleteAsync($"{address_GetStudents}/{student1.Id}");
                    if (!deleteResponse.IsSuccessStatusCode)
                    {
                        TempData["ErrorMessage"] = "Failed to delete student from database.";
                        return RedirectToAction("Index");
                    }
                }
            }

            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> DeleteConfirmed(int studentId)
        {
            List<Students> students = new List<Students>();
            HttpResponseMessage response = _httpClient.GetAsync(address_GetStudents).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                students = JsonConvert.DeserializeObject<List<Students>>(data);
            }
            foreach (var student in students)
            {
                if (student.Id == studentId)
                {
                    return View(student);
                }
            }
            TempData["ErrorMessage"] = "Failed to Find the Student";
            return RedirectToAction("Index");
        }

        //[HttpGet]
        //[Authorize(Roles = "Student")]
        //public async Task<IActionResult> RegesteredCourses()
        //{
        //    var UserId = _userManager.GetUserId(User);
        //    if (UserId == null)
        //        return RedirectToAction("Index");

        //    List<Students> students = new List<Students>();
        //    HttpResponseMessage response = _httpClient.GetAsync(address_GetStudents).Result;
        //    if (response.IsSuccessStatusCode)
        //    {
        //        string data = response.Content.ReadAsStringAsync().Result;
        //        students = JsonConvert.DeserializeObject<List<Students>>(data);
        //    }

        //    foreach (var student in students)
        //    {
        //        if (student.UserId == UserId)
        //        {
        //            foreach (var course in student.courses) {
        //                Debug.WriteLine(course.TeacherId + " The name is " + course.Teacher.Teacher_Name + " " + course.Teacher.Email);
        //            }

        //            return View(student);
        //        }
        //    }
        //    return RedirectToAction("Index");
        //}
        [HttpGet]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> RegesteredCourses(string sortField = "Course_Name", string sortOrder = "asc")
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return RedirectToAction("Index");

            List<Students> students = new List<Students>();
            HttpResponseMessage response = await _httpClient.GetAsync(address_GetStudents);
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                students = JsonConvert.DeserializeObject<List<Students>>(data);
            }

            foreach (var student in students)
            {
                if (student.UserId == userId)
                {
                    
                    if (student.courses != null)
                    {
                        student.courses = student.courses
                        .Where(c => c.isDone == 0 || c.isDone==null) // only unfinished courses
                        .ToList();

                        switch (sortField)
                        {
                            case "Course_Name":
                                student.courses = sortOrder == "desc"
                                    ? student.courses.OrderByDescending(c => c.Course_Name).ToList()
                                    : student.courses.OrderBy(c => c.Course_Name).ToList();
                                break;

                            case "Id":
                                student.courses = sortOrder == "desc"
                                    ? student.courses.OrderByDescending(c => c.Id).ToList()
                                    : student.courses.OrderBy(c => c.Id).ToList();
                                break;

                            case "Teacher_Name":
                                student.courses = sortOrder == "desc"
                                    ? student.courses.OrderByDescending(c => c.Teacher.Teacher_Name).ToList()
                                    : student.courses.OrderBy(c => c.Teacher.Teacher_Name).ToList();
                                break;
                        }
                    }

                    return View(student);
                }
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditStudentAdmin(int id)
        {
            var student = await _httpClient.GetFromJsonAsync<Students>($"http://localhost:5134/api/Students/{id}");

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditStudentAdmin(int id, string name, string email, int hours_term, double reciept)
        {
            var student = await _httpClient.GetFromJsonAsync<Students>($"http://localhost:5134/api/Students/{id}");

            if (student == null)
            {
                TempData["ErrorMessage"] = "Student couldnt be found";
                return RedirectToAction("Index");
            }
            var EmailUser = student.Email;
            var user = await _userManager.FindByEmailAsync(EmailUser);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User couldnt be found";
                return RedirectToAction("Index");
            }
            var putUrl = $"http://localhost:5134/api/Students/{id}/Admin?Name={name}&Email={email}&hours_term={hours_term}&reciept={reciept}";
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
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> EditStudent(int id)
        {
            var student = await _httpClient.GetFromJsonAsync<Students>($"http://localhost:5134/api/Students/{id}");

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }
        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> EditStudent(int id, string name, string email, string Phone)
        {
            var student = await _httpClient.GetFromJsonAsync<Students>($"http://localhost:5134/api/Students/{id}");

            if (student == null)
            {
                TempData["ErrorMessage"] = "Student couldnt be found";
                return RedirectToAction("ShowProfile");
            }
            var EmailUser = student.Email;
            var user = await _userManager.FindByEmailAsync(EmailUser);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User couldnt be found";
                return RedirectToAction("ShowProfile");
            }
            var putUrl = $"http://localhost:5134/api/Students/{id}/Profile?Name={name}&Email={email}&Phone={Phone}";
            var putResponse = await _httpClient.PutAsync(putUrl, null);
            if (!putResponse.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Failed to Edit values.";
                return RedirectToAction("ShowProfile");
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
            return RedirectToAction("ShowProfile");
        }
        [HttpGet]
        [Authorize(Roles ="Teacher")]
        public async Task<IActionResult> EditGrade(int studentId, int courseId)
        {
            var studentGrade = await _httpClient.GetFromJsonAsync<Grades>($"http://localhost:5134/api/Students/{studentId}/{courseId}/GetGrade");

            if (studentGrade == null)
            {
                TempData["ErrorMessage"] = "Student couldnt be found";
                return RedirectToAction("StudentsinCourse", "Teachers");
            }
            return View(studentGrade);
            
        }
        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> EditGrade(int studentId, int courseId, int first, int second, int final)
        {
            Students student = new Students();
            HttpResponseMessage response = _httpClient.GetAsync($"{address_GetStudents}/{studentId}").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                student = JsonConvert.DeserializeObject<Students>(data);
            }

            if (student == null)
            {
                TempData["ErrorMessage"] = "Student couldnt be found";
                return RedirectToAction("StudentsinCourse", "Teachers");
            }

            foreach (var course in student.courses)
            {
                if (course.Id == courseId)
                {
                    var putUrl = $"http://localhost:5134/api/Students/{studentId}/{courseId}/Grade?first={first}&second={second}&final={final}";
                    var putResponse = await _httpClient.PutAsync(putUrl, null);
                    if (!putResponse.IsSuccessStatusCode)
                    {
                        TempData["ErrorMessage"] = "Failed to Edit values: put response failed.";
                        return RedirectToAction("StudentsinCourse", "Teachers");
                    }
                }
            }
            return RedirectToAction("StudentsinCourse", "Teachers", new { courseId = courseId });

        }
        [HttpGet]
        public async Task<IActionResult> FinishedCourses(string sortField="Course_Name",string sortOrder="asc")
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return RedirectToAction("Index");

            List<Students> students = new List<Students>();
            HttpResponseMessage response = await _httpClient.GetAsync(address_GetStudents);
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                students = JsonConvert.DeserializeObject<List<Students>>(data);
            }

            foreach (var student in students)
            {
                if (student.UserId == userId)
                {

                    if (student.courses != null)
                    {
                        student.courses = student.courses
                        .Where(c => c.isDone == 1) // only unfinished courses
                        .ToList();

                        switch (sortField)
                        {
                            case "Course_Name":
                                student.courses = sortOrder == "desc"
                                    ? student.courses.OrderByDescending(c => c.Course_Name).ToList()
                                    : student.courses.OrderBy(c => c.Course_Name).ToList();
                                break;

                            case "Id":
                                student.courses = sortOrder == "desc"
                                    ? student.courses.OrderByDescending(c => c.Id).ToList()
                                    : student.courses.OrderBy(c => c.Id).ToList();
                                break;

                            case "Teacher_Name":
                                student.courses = sortOrder == "desc"
                                    ? student.courses.OrderByDescending(c => c.Teacher.Teacher_Name).ToList()
                                    : student.courses.OrderBy(c => c.Teacher.Teacher_Name).ToList();
                                break;
                        }
                    }

                    return View(student);
                }
            }

            return RedirectToAction("Index");
        }
    }
}
