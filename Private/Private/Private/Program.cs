var builder = WebApplication.CreateBuilder(args);

// ── Register ALL services BEFORE app.Build() ──
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

// ── HTTP Client for API ──
builder.Services.AddHttpClient("API", c =>
{
    c.BaseAddress = new Uri("https://localhost:7108/api/");
    c.DefaultRequestHeaders.Add("Accept", "application/json");
}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback =
        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
});

// ── Session ──
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// ── Configure middleware AFTER app.Build() ──
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapRazorPages();

app.Run();