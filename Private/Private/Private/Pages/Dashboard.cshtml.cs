using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Private.Pages
{
    public class DashboardModel : PageModel
    {
        private readonly HttpClient _http;
        private static readonly JsonSerializerOptions _json = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public DashboardModel(IHttpClientFactory factory)
        {
            _http = factory.CreateClient("API");
        }

        public List<EventDto> Events { get; set; } = new();
        public string? Error { get; set; }
        public string? Success { get; set; }
        public int OrganiserId { get; set; }
        public string OrganiserName { get; set; } = "";

        public async Task<IActionResult> OnGetAsync()
        {
            var role = HttpContext.Session.GetString("kmc_user_role");
            if (role != "organiser") return RedirectToPage("/Login");

            OrganiserId = int.Parse(HttpContext.Session.GetString("kmc_user_id") ?? "0");
            OrganiserName = HttpContext.Session.GetString("kmc_user_name") ?? "";

            await LoadEvents();
            return Page();
        }

        public async Task<IActionResult> OnPostCreateAsync(
            string title, string description, string startDate, string endDate,
            string eventType, string location, int maxParticipants, string status)
        {
            OrganiserId = int.Parse(HttpContext.Session.GetString("kmc_user_id") ?? "0");
            OrganiserName = HttpContext.Session.GetString("kmc_user_name") ?? "";

            var body = new
            {
                title,
                description,
                startDate = DateTime.Parse(startDate),
                endDate = DateTime.Parse(endDate),
                eventType,
                location,
                maxParticipants,
                status,
                organiserId = OrganiserId
            };

            var res = await _http.PostAsJsonAsync("Event/private", body);
            Success = res.IsSuccessStatusCode ? "Event created successfully!" : "Failed to create event.";

            await LoadEvents();
            return Page();
        }

        public async Task<IActionResult> OnPostEditAsync(
            int id, string title, string description, string startDate, string endDate,
            string eventType, string location, int maxParticipants, string status)
        {
            OrganiserId = int.Parse(HttpContext.Session.GetString("kmc_user_id") ?? "0");
            OrganiserName = HttpContext.Session.GetString("kmc_user_name") ?? "";

            var body = new
            {
                title,
                description,
                startDate = DateTime.Parse(startDate),
                endDate = DateTime.Parse(endDate),
                eventType,
                location,
                maxParticipants,
                status,
                organiserId = OrganiserId
            };

            var res = await _http.PutAsJsonAsync($"Event/{id}?organiserId={OrganiserId}", body);
            Success = res.IsSuccessStatusCode ? "Event updated successfully!" : "Failed to update event.";

            await LoadEvents();
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            OrganiserId = int.Parse(HttpContext.Session.GetString("kmc_user_id") ?? "0");
            OrganiserName = HttpContext.Session.GetString("kmc_user_name") ?? "";

            var res = await _http.DeleteAsync($"Event/{id}?organiserId={OrganiserId}");
            Success = res.IsSuccessStatusCode ? "Event deleted." : "Failed to delete event.";

            await LoadEvents();
            return Page();
        }

        private async Task LoadEvents()
        {
            try
            {
                var res = await _http.GetAsync("Event/private");
                if (res.IsSuccessStatusCode)
                {
                    var all = await res.Content.ReadFromJsonAsync<List<EventDto>>(_json) ?? new();
                    Events = all.Where(e => e.OrganiserId == OrganiserId).ToList();
                }
                else Error = "Failed to load events.";
            }
            catch { Error = "Could not connect to server."; }
        }

        public class EventDto
        {
            public int Id { get; set; }
            public string Status { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string EventType { get; set; }
            public string Location { get; set; }
            public int MaxParticipants { get; set; }
            public int CurrentParticipants { get; set; }
            public string WebsiteFilter { get; set; }
            public int OrganiserId { get; set; }
            public string OrganiserName { get; set; }
        }
    }
}