
using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Services;
using RaceDay.Web.ViewModels;
using System.Net.Http.Json;

namespace RaceDay.Web.Controllers
{
    public class EventsController : Controller
    {
        private readonly RaceDayApiService _apiService;

        public EventsController(RaceDayApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var client = _apiService.CreateClient();

            var response = await client.GetAsync("api/Events");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage =
                    "Unable to load events. Please try again later.";

                return View(new List<EventViewModel>());
            }

            var events = await response.Content
                .ReadFromJsonAsync<List<EventViewModel>>();

            return View(events ?? new List<EventViewModel>());
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Login", "Account");

            var client = _apiService.CreateClient();

            var response = await client.GetAsync($"api/Events/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var eventItem = await response.Content.ReadFromJsonAsync<EventViewModel>();

            if (eventItem == null)
                return NotFound();

            var model = new CreateEventViewModel
            {
                Name = eventItem.Name,
                Description = eventItem.Description,
                EventDate = eventItem.EventDate,
                Location = eventItem.Location,
                Distance = eventItem.Distance,
                EventType = eventItem.EventType
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreateEventViewModel model)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = _apiService.CreateClient();

            var eventData = new
            {
                model.Name,
                model.Description,
                model.EventDate,
                model.Location,
                model.Distance,
                model.EventType
            };

            var response = await client.PutAsJsonAsync($"api/Events/{id}", eventData);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to update the event. Please check your details and try again."
                );

                return View(model);
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
            {
                return RedirectToAction("Login", "Account");
            }

            var client = _apiService.CreateClient();

            var response = await client.DeleteAsync($"api/Events/{id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] =
                    "Unable to delete the event. Please try again.";
            }

            return RedirectToAction("Index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEventViewModel model)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = _apiService.CreateClient();

            var eventData = new
            {
                model.Name,
                model.Description,
                model.EventDate,
                model.Location,
                model.Distance,
                model.EventType
            };

            var response = await client.PostAsJsonAsync("api/Events", eventData);

            if (!response.IsSuccessStatusCode)
{
    var errorMessage = await response.Content.ReadAsStringAsync();

    ModelState.AddModelError(
        string.Empty,
        $"API Error: {(int)response.StatusCode} - {errorMessage}"
    );

    return View(model);
}

            return RedirectToAction("Index");
        }

    }
}


