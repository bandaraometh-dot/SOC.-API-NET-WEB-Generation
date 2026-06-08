using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Private.Pages
{
    public class LoginModel : PageModel
    {
        private readonly HttpClient _http;
        private static readonly JsonSerializerOptions _json = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public LoginModel(IHttpClientFactory factory)
        {
            _http = factory.CreateClient("API");
        }

        [BindProperty] public string Username { get; set; }
        [BindProperty] public string Password { get; set; }
        public string? Error { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostCitizenAsync()
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                Error = "Please fill in all fields.";
                return Page();
            }

            var res = await _http.GetAsync("Citizen");
            var data = await res.Content.ReadFromJsonAsync<List<CitizenDto>>(_json);
            var user = data?.Find(u =>
                string.Equals(u.Username, Username, StringComparison.OrdinalIgnoreCase) &&
                u.Password == Password);

            if (user == null) { Error = "Invalid username or password."; return Page(); }

            HttpContext.Session.SetString("kmc_user_id", user.Id.ToString());
            HttpContext.Session.SetString("kmc_user_name", user.FullName);
            HttpContext.Session.SetString("kmc_user_role", "citizen");
            return RedirectToPage("/Events");
        }

        public async Task<IActionResult> OnPostOrganiserAsync()
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                Error = "Please fill in all fields.";
                return Page();
            }

            var res = await _http.GetAsync("Organiser");
            var data = await res.Content.ReadFromJsonAsync<List<OrganiserDto>>(_json);
            var user = data?.Find(u =>
                string.Equals(u.Username, Username, StringComparison.OrdinalIgnoreCase) &&
                u.Password == Password);

            if (user == null) { Error = "Invalid username or password."; return Page(); }

            HttpContext.Session.SetString("kmc_user_id", user.Id.ToString());
            HttpContext.Session.SetString("kmc_user_name", user.Name);
            HttpContext.Session.SetString("kmc_user_role", "organiser");
            return RedirectToPage("/Dashboard");
        }

        public class CitizenDto
        {
            public int Id { get; set; }
            public string FullName { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
        }

        public class OrganiserDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}