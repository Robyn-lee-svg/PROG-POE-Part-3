using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Services;
using RaceDay.Web.ViewModels;
using System.Net.Http.Json;

namespace RaceDay.Web.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly RaceDayApiService _apiService;

        public CategoriesController(RaceDayApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
            {
                return RedirectToAction("Login", "Account");
            }

            var client = _apiService.CreateClient();

            var response = await client.GetAsync("api/Categories");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage =
                    "Unable to load categories. Please try again later.";

                return View(new List<CategoryViewModel>());
            }

            var categories = await response.Content
                .ReadFromJsonAsync<List<CategoryViewModel>>();

            return View(categories ?? new List<CategoryViewModel>());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Login", "Account");

            var client = _apiService.CreateClient();

            var response = await client.DeleteAsync($"api/Categories/{id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] =
                    "Unable to delete the category. Please try again.";
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditCategoryViewModel model)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(model);

            var client = _apiService.CreateClient();

            var categoryData = new
            {
                model.CategoryID,
                model.EventID,
                model.CategoryName,
                model.Description
            };

            var response = await client.PutAsJsonAsync(
                $"api/Categories/{model.CategoryID}", categoryData);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty,
                    "Unable to update the category. Please try again.");

                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Login", "Account");

            var client = _apiService.CreateClient();

            var response = await client.GetAsync($"api/Categories/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var category = await response.Content
                .ReadFromJsonAsync<CategoryViewModel>();

            if (category == null)
                return NotFound();

            var model = new EditCategoryViewModel
            {
                CategoryID = category.CategoryID,
                EventID = category.EventID,
                CategoryName = category.CategoryName,
                Description = category.Description
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCategoryViewModel model)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Login", "Account");

            var userId = HttpContext.Session.GetInt32("UserID");
            var client = _apiService.CreateClient();

            
            var eventsResponse = await client.GetAsync("api/Events");

            if (!eventsResponse.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage = "Unable to load your events. Please try again.";
                ViewBag.Events = new List<EventViewModel>();
                return View(model);
            }

            var events = await eventsResponse.Content
                .ReadFromJsonAsync<List<EventViewModel>>();

            var organiserEvents = events?
                .Where(e => e.OrganiserID == userId)
                .ToList() ?? new List<EventViewModel>();

            ViewBag.Events = organiserEvents;

            if (!ModelState.IsValid)
                return View(model);

            
            if (!organiserEvents.Any(e => e.EventID == model.EventID))
            {
                ModelState.AddModelError("EventID",
                    "Please select one of your own events.");
                return View(model);
            }

            var categoryData = new
            {
                model.EventID,
                model.CategoryName,
                model.Description
            };

            var response = await client.PostAsJsonAsync(
                "api/Categories", categoryData);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty,
                    "The category could not be created. Please try again.");
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = HttpContext.Session.GetInt32("UserID");

            var client = _apiService.CreateClient();

            var response = await client.GetAsync("api/Events");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage = "Unable to load your events. Please try again.";
                ViewBag.Events = new List<EventViewModel>();
                return View(new CreateCategoryViewModel());
            }

            var events = await response.Content
                .ReadFromJsonAsync<List<EventViewModel>>();

            var organiserEvents = events?
                .Where(e => e.OrganiserID == userId)
                .ToList() ?? new List<EventViewModel>();

            ViewBag.Events = organiserEvents;

            return View(new CreateCategoryViewModel());
        }
    }
}
