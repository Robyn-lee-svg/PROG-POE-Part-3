using System.Net;
using System.Net.Http;

namespace RaceDay.Web.Services
{
    public class RaceDayApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RaceDayApiService(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public HttpClient CreateClient()
        {
            var client = _httpClientFactory.CreateClient("RaceDayApi");

            var apiCookie = _httpContextAccessor.HttpContext?
                .Session.GetString("ApiSessionCookie");

            if (!string.IsNullOrEmpty(apiCookie))
            {
                client.DefaultRequestHeaders.Remove("Cookie");
                client.DefaultRequestHeaders.Add("Cookie", apiCookie);
            }

            return client;
        }

        public async Task SaveApiSessionCookieAsync(
            HttpResponseMessage response)
        {
            if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
            {
                var sessionCookie = cookies.FirstOrDefault(c =>
                    c.StartsWith(".AspNetCore.Session=", StringComparison.OrdinalIgnoreCase));

                if (sessionCookie != null)
                {
                    var cookieValue = sessionCookie.Split(';')[0];

                    _httpContextAccessor.HttpContext?
                        .Session.SetString("ApiSessionCookie", cookieValue);
                }
            }
        }
    }
}