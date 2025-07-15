using CityBonesPortfolio.Models;
using CityBonesPortfolio.Repositories;

var builder = WebApplication.CreateBuilder(args);

// -------------------------
// Load AppSettings section
// -------------------------
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

// -------------------------
// Dependency Injection
// -------------------------
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IMarketRepository, MarketRepository>();
builder.Services.AddScoped<IProductRepository>(sp =>
    new ProductRepository(builder.Configuration.GetConnectionString("citybones")));
builder.Services.AddScoped<IUserProductRepository, UserProductRepository>();


// -------------------------
// Session Middleware
// -------------------------
builder.Services.AddDistributedMemoryCache(); // Required for session

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// -------------------------
// Authentication & Authorization
// -------------------------
builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// -------------------------
// HTTP request pipeline
// -------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession(); // 👈 Required for session support

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
