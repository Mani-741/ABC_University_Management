using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityAcademicManagementSystem.Models;
using UniversityAcademicManagementSystem.Services.Interfaces;

namespace UniversityAcademicManagementSystem.Controllers
{
    public class HomeController : Controller
    {

        private readonly IUserService _userService;
        private readonly IStudentService _studentService;
        private readonly IRegistrarService _registrarService;

        public HomeController(IUserService userService, IStudentService studentService, IRegistrarService registrarService)
        {
            _userService = userService;
            _studentService = studentService;
            _registrarService = registrarService;
        }

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin")) return RedirectToAction("Index", "Admin");
                if (User.IsInRole("Student")) return RedirectToAction("Index", "Student");
                if (User.IsInRole("Registrar")) return RedirectToAction("Index", "Registrar");
                if (User.IsInRole("Faculty")) return RedirectToAction("Index", "Faculty");
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _userService.RegisterUserAsync(model);

            if (result)
            {
                TempData["Success"] = "Registration Successful! Please Login.";
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("Email", "This email is already registered.");
            return View(model);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Email and Password are required.");
                return View();
            }

            var user = await _userService.LoginUserAsync(email, password);

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.ToString()),
                    new Claim("UserId", user.UserId.ToString())
                };

                if (user.Role == Role.Faculty || user.Role == Role.Registrar)
                {
                    claims.Add(new Claim("Department", user.Department ?? ""));
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

                if (user.Role == Role.Admin)
                {
                    return RedirectToAction("Index", "Admin");
                }

                if(user.Role == Role.Faculty)
                {
                    return RedirectToAction("Index", "Faculty");
                }

                if(user.Role == Role.Registrar)
                {
                    return RedirectToAction("Index", "Registrar");
                }

                if(user.Role == Role.Student)
                {
                    var isProfileComplete = await _studentService.IsProfileCompleteAsync(user.Email);

                    if (!isProfileComplete)
                    {
                        TempData["Info"] = "Please complete your profile to continue!";
                        return RedirectToAction("CompleteProfile", "Student");
                    }

                    return RedirectToAction("Index", "Student");
                }

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid Email or Password.");
            return View();
        }

        [HttpPost]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            TempData.Clear();

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Error(int? statusCode = null)
        {
            return View();
        }

        public async Task<IActionResult> Courses()
        {
            var courses = await _registrarService.GetAllCoursesAsync();

            return View(courses);
        }

	

		[Authorize] 
		[HttpGet]
		public IActionResult ChangePassword()
		{
			return View();
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
		{
			var email = User.Identity?.Name;
			if (string.IsNullOrEmpty(email)) return RedirectToAction("Login");

			
			if (newPassword != confirmPassword)
			{
				ModelState.AddModelError("", "New passwords do not match.");
				return View();
			}

			var passwordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$");

			if (string.IsNullOrEmpty(newPassword) || newPassword.Length < 8)
			{
				ModelState.AddModelError("", "Password must be at least 8 characters long.");
			}
			else if (!passwordRegex.IsMatch(newPassword))
			{
				ModelState.AddModelError("", "Password must contain an uppercase letter, lowercase letter, number, and special character.");
			}

			if (newPassword != confirmPassword)
			{
				ModelState.AddModelError("", "New passwords do not match.");
			}

			
			if (!ModelState.IsValid)
			{
				return View();
			}

			
			var user = await _userService.LoginUserAsync(email, currentPassword);
			if (user == null)
			{
				ModelState.AddModelError("", "Current password is incorrect.");
				return View();
			}

			
			var result = await _userService.UpdatePasswordAsync(user.UserId, newPassword);
			if (result)
			{
				TempData["Success"] = "Password updated successfully!";

				
				if (User.IsInRole("Admin")) return RedirectToAction("Index", "Admin");
				if (User.IsInRole("Faculty")) return RedirectToAction("Index", "Faculty");
				if (User.IsInRole("Registrar")) return RedirectToAction("Index", "Registrar");
				if (User.IsInRole("Student")) return RedirectToAction("Index", "Student");

				return RedirectToAction("Index", "Home");
			}

			ModelState.AddModelError("", "An error occurred while updating the password.");
			return View();
		}
	}
}
