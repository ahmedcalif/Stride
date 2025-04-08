using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Stride.Data.Models;
using Stride.Data.Models.SQLRepository;
using Microsoft.AspNetCore.Identity;
using Stride.Data.Data;
using Stride.Data.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

// Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
// Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
// Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Staging");
// Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Production");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
Console.WriteLine($"Current environment: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}");

if (builder.Environment.IsProduction() || builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<IUserRepository, SQLUserRepository>();
    builder.Services.AddScoped<IHabitRepository, SQLHabitRepository>();
    builder.Services.AddScoped<IGoalRepository, SQLGoalRepository>();
    Console.WriteLine("Using DB repositories");
}

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configure the database context
builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();


builder.Services.AddAuthentication()
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
        googleOptions.CallbackPath = "/signin-google";
    });

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    
    options.ReturnUrlParameter = "returnUrl";
    options.Events = new CookieAuthenticationEvents
    {
        OnRedirectToLogin = context =>
        {
            context.RedirectUri = "/Identity/Account/Login";
            return Task.CompletedTask;
        },
        OnRedirectToLogout = context =>
        {
            context.RedirectUri = "/Identity/Account/Logout";
            return Task.CompletedTask;
        },
        OnRedirectToAccessDenied = context =>
        {
            context.RedirectUri = "/Identity/Account/AccessDenied";
            return Task.CompletedTask;
        },
        OnRedirectToReturnUrl = context =>
        {
            if (string.IsNullOrEmpty(context.RedirectUri) || context.RedirectUri == "/")
            {
                context.RedirectUri = "/Dashboard/Index";
            }
            return Task.CompletedTask;
        }
    };
});


builder.Services.AddMvc(config => {
    var policy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();
    config.Filters.Add(new AuthorizeFilter(policy));
});

builder.WebHost.UseUrls("http://*:8080");

var app = builder.Build();
app.MapGet("/health", () => "Healthy");

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Testing"))
{
    app.UseDeveloperExceptionPage();
    Console.WriteLine($"Using Developer Exception Page in {app.Environment.EnvironmentName} environment");
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    Console.WriteLine("Using Production error handling with HSTS");
}

app.UseStatusCodePages(async context => {
    var response =  context.HttpContext.Response;
    
    if (response.StatusCode == 404)
    {
        response.Redirect($"/Error/HandleStatusCode/{response.StatusCode}");
    }
});

app.UseStaticFiles();
app.UseSession();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

try
{
    Stride.Data.DbInitializer.Initialize(app.Services);
    Console.WriteLine("Database initialized successfully.");
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred while initializing the database: {ex.Message}");
}

// Run the app
app.Run();