using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Services;
using RaceDay.Web.ViewModels;
using System.Net.Http.Json;

namespace RaceDay.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly RaceDayApiService _apiService;

        public AccountController(RaceDayApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = _apiService.CreateClient();

            var response = await client.PostAsJsonAsync(
                "api/Auth/login",
                new
                {
                    model.Email,
                    model.Password
                });

            await _apiService.SaveApiSessionCookieAsync(response);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid email or password. Please try again.");

                return View(model);
            }

            var user = await response.Content.ReadFromJsonAsync<UserLoginResponse>();

            if (user == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to retrieve your account details.");

                return View(model);
            }

            HttpContext.Session.SetInt32("UserID", user.UserID);
            HttpContext.Session.SetString("FirstName", user.FirstName ?? "");
            HttpContext.Session.SetString("LastName", user.LastName ?? "");
            HttpContext.Session.SetString("Email", user.Email ?? "");
            HttpContext.Session.SetString("Role", user.Role ?? "");

            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = _apiService.CreateClient();

            var response = await client.PostAsJsonAsync(
                "api/Auth/register",
                new
                {
                    model.FirstName,
                    model.LastName,
                    model.Email,
                    model.Password,
                    model.Role,
                    model.Phone
                });

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    ModelState.AddModelError(
                        "Email",
                        "An account with this email already exists.");
                }
                else
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Registration failed. Please check your details and try again.");
                }

                return View(model);
            }

            TempData["SuccessMessage"] =
                "Your account was created successfully. Please log in.";

            return RedirectToAction("Login");
        }

    }


    public class UserLoginResponse
    {
        public int UserID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
    }
}