using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using System.Text.Json;

namespace Private.Pages
{
    public class EventsModel : PageModel
    {
        private readonly HttpClient _http;
        private static readonly JsonSerializerOptions _json = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public EventsModel(IHttpClientFactory factory)
        {
            _http = factory.CreateClient("API");
        }

        public List<EventDto> Events { get; set; } = new();
        public List<string> EventTypes { get; set; } = new();
        public string? Error { get; set; }

        [BindProperty(SupportsGet = true)] public string? Search { get; set; }
        [BindProperty(SupportsGet = true)] public string? FilterType { get; set; }
        [BindProperty(SupportsGet = true)] public string? FilterDate { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                // ── fetch only private web events ──
                var res = await _http.GetAsync("Event/private");
                if (!res.IsSuccessStatusCode) { Error = "Failed to load events."; return; }

                var all = await res.Content.ReadFromJsonAsync<List<EventDto>>(_json) ?? new();

                EventTypes = all.Select(e => e.EventType)
                                .Where(t => !string.IsNullOrEmpty(t))
                                .Distinct().OrderBy(t => t).ToList();

                if (!string.IsNullOrEmpty(Search))
                    all = all.Where(e =>
                        (e.Title ?? "").Contains(Search, StringComparison.OrdinalIgnoreCase) ||
                        (e.Location ?? "").Contains(Search, StringComparison.OrdinalIgnoreCase) ||
                        (e.Description ?? "").Contains(Search, StringComparison.OrdinalIgnoreCase)
                    ).ToList();

                if (!string.IsNullOrEmpty(FilterType))
                    all = all.Where(e =>
                        string.Equals(e.EventType, FilterType, StringComparison.OrdinalIgnoreCase)
                    ).ToList();

                if (!string.IsNullOrEmpty(FilterDate) && DateTime.TryParse(FilterDate, out var date))
                    all = all.Where(e => e.StartDate.Date == date.Date).ToList();

                Events = all;
            }
            catch
            {
                Error = "Could not connect to the server.";
            }
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
            public DateTime CreatedAt { get; set; }
            public string WebsiteFilter { get; set; }
            public int OrganiserId { get; set; }
            public string OrganiserName { get; set; }
        }
    }
}