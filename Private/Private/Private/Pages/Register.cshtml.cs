using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Private.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly HttpClient _http;
        private static readonly JsonSerializerOptions _json = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public RegisterModel(IHttpClientFactory factory)
        {
            _http = factory.CreateClient("API");
        }

        [BindProperty] public string FullName { get; set; }
        [BindProperty] public string Name { get; set; }
        [BindProperty] public string Username { get; set; }
        [BindProperty] public string Email { get; set; }
        [BindProperty] public string Phone { get; set; }
        [BindProperty] public string Password { get; set; }
        [BindProperty] public string Organisation { get; set; }
        public string? Error { get; set; }
        public string? Success { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostCitizenAsync()
        {
            if (string.IsNullOrEmpty(FullName) || string.IsNullOrEmpty(Username) ||
                string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Phone) ||
                string.IsNullOrEmpty(Password))
            {
                Error = "Please fill in all fields.";
                return Page();
            }

            var body = new { fullName = FullName, username = Username, email = Email, phone = Phone, password = Password };
            var res = await _http.PostAsJsonAsync("Citizen", body);

            if (res.IsSuccessStatusCode || res.StatusCode == System.Net.HttpStatusCode.Created)
            {
                Success = "Citizen account created! Redirecting to login...";
                return Page();
            }

            Error = "Registration failed. Username or email may already be in use.";
            return Page();
        }

        public async Task<IActionResult> OnPostOrganiserAsync()
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Username) ||
                string.IsNullOrEmpty(Organisation) || string.IsNullOrEmpty(Email) ||
                string.IsNullOrEmpty(Phone) || string.IsNullOrEmpty(Password))
            {
                Error = "Please fill in all fields.";
                return Page();
            }

            var body = new { name = Name, username = Username, organisation = Organisation, email = Email, phone = Phone, password = Password };
            var res = await _http.PostAsJsonAsync("Organiser", body);

            if (res.IsSuccessStatusCode || res.StatusCode == System.Net.HttpStatusCode.Created)
            {
                Success = "Organiser account created! Redirecting to login...";
                return Page();
            }

            Error = "Registration failed. Username or email may already be in use.";
            return Page();
        }
    }
}