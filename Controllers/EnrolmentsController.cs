using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Services;
using RaceDay.Web.ViewModels;
using System.Net.Http.Json;

namespace RaceDay.Web.Controllers
{
    public class EnrolmentsController : Controller
    {
        private readonly RaceDayApiService _apiService;

        public EnrolmentsController(RaceDayApiService apiService)
        {
            _apiService = apiService;
        }



        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("Role") != "Participant")
                return RedirectToAction("Login", "Account");

            var client = _apiService.CreateClient();

            var response = await client.GetAsync("api/Enrolments/MyEnrolments");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage = "Unable to load your enrolled events.";
                return View(new List<MyEnrolmentViewModel>());
            }

            var enrolments = await response.Content
                .ReadFromJsonAsync<List<MyEnrolmentViewModel>>();

            return View(enrolments ?? new List<MyEnrolmentViewModel>());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEnrolmentViewModel model)
        {
            if (HttpContext.Session.GetString("Role") != "Participant")
                return RedirectToAction("Login", "Account");

            var client = _apiService.CreateClient();

           
            var eventResponse = await client.GetAsync(
                $"api/Events/{model.EventID}");

            if (!eventResponse.IsSuccessStatusCode)
                return NotFound();

            var eventItem = await eventResponse.Content
                .ReadFromJsonAsync<EventViewModel>();

            if (eventItem == null)
                return NotFound();

            
            var categoriesResponse = await client.GetAsync(
                $"api/Categories/ByEvent/{model.EventID}");

            var categories = categoriesResponse.IsSuccessStatusCode
                ? await categoriesResponse.Content
                    .ReadFromJsonAsync<List<CategoryViewModel>>()
                : new List<CategoryViewModel>();

            ViewBag.Event = eventItem;
            ViewBag.Categories = categories ?? new List<CategoryViewModel>();

            if (!ModelState.IsValid)
                return View(model);

            var enrolmentData = new
            {
                model.EventID,
                model.CategoryID
            };

            var response = await client.PostAsJsonAsync(
                "api/Enrolments", enrolmentData);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to complete enrolment. You may already be enrolled in this event.");

                return View(model);
            }

            TempData["SuccessMessage"] = "You have successfully enrolled in the event!";

            return RedirectToAction("Index", "Events");
        }

        [HttpGet]
        public async Task<IActionResult> Create(int eventId)
        {
            if (HttpContext.Session.GetString("Role") != "Participant")
                return RedirectToAction("Login", "Account");

            var client = _apiService.CreateClient();

            var eventResponse = await client.GetAsync($"api/Events/{eventId}");

            if (!eventResponse.IsSuccessStatusCode)
                return NotFound();

            var eventItem = await eventResponse.Content
                .ReadFromJsonAsync<EventViewModel>();

            if (eventItem == null)
                return NotFound();

            var categoriesResponse = await client.GetAsync(
                $"api/Categories/ByEvent/{eventId}");

            if (!categoriesResponse.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage = "Unable to load event categories.";
                ViewBag.Event = eventItem;
                ViewBag.Categories = new List<CategoryViewModel>();
                return View(new CreateEnrolmentViewModel { EventID = eventId });
            }

            var categories = await categoriesResponse.Content
                .ReadFromJsonAsync<List<CategoryViewModel>>();

            ViewBag.Event = eventItem;
            ViewBag.Categories = categories ?? new List<CategoryViewModel>();

            return View(new CreateEnrolmentViewModel { EventID = eventId });
        }
        [HttpGet]
        public async Task<IActionResult> OrganiserEnrolments()
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Login", "Account");

            var client = _apiService.CreateClient();

            var response = await client.GetAsync(
                "api/Enrolments/OrganiserEnrolments");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage = "Unable to load event enrolments.";
                return View(new List<OrganiserEnrolmentViewModel>());
            }

            var enrolments = await response.Content
                .ReadFromJsonAsync<List<OrganiserEnrolmentViewModel>>();

            return View(enrolments ?? new List<OrganiserEnrolmentViewModel>());
        }


    }

}
