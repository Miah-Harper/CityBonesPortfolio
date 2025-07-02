using CityBonesPortfolio.Models;

var builder = WebApplication.CreateBuilder(args);


 static void Main(string[] args)
{
    CreateHostBuilder(args).Build().Run();
}

    static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
            config.SetBasePath(Directory.GetCurrentDirectory());
            config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        })
        .ConfigureServices((hostContext, services) =>
        {
                // Build configuration
            var configuration = hostContext.Configuration;

                // Configure AppSettings
            services.Configure<AppSettings>(configuration.GetSection("AppSettings"));

                // Add other services here if needed
           
        });

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
});


builder.Services.AddAuthorization();

builder.Services.AddSession();

void ConfigureServices(IServiceCollection services)
{
    services.AddControllersWithViews();

    services.AddDistributedMemoryCache(); // Required for session



    services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(30);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });
}

void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    // other middleware

    app.UseRouting();

    app.UseSession(); // enable session middleware

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapDefaultControllerRoute();
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
