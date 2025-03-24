using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Stride.Data.Data;
using Stride.Data.Models;
using Stride.Data.Models.SQLRepository;
using Microsoft.AspNetCore.Identity;
using Stride.Data;
using Stride.Data.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

// Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
// Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
// Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Staging");
Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Production");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
Console.WriteLine($"Current environment: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}");


if (builder.Environment.IsDevelopment() || builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddSingleton<IUserRepository, MockUserRepository>();
    builder.Services.AddSingleton<IGoalRepository, MockGoalRepository>();
    builder.Services.AddSingleton<IHabitRepository, MockHabitRepository>();
    Console.WriteLine($"Using Mock Repositories in {builder.Environment.EnvironmentName} environment");
}
else
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

builder.Services.AddDbContext<Stride.Data.Data.ApplicationDBContext>(options => 
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));


builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    
    options.User.RequireUniqueEmail = false;
    // To disable the identity password checker
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 1; 
})
.AddEntityFrameworkStores<Stride.Data.Data.ApplicationDBContext>()
.AddDefaultTokenProviders();
builder.Services.AddScoped<IPasswordValidator<ApplicationUser>, CustomPasswordValidator<ApplicationUser>>();

builder.Services.AddTransient<Stride.Data.Services.IEmailSender, EmailSender>();

builder.Services.Configure<IdentityOptions>(options =>
{

    options.User.RequireUniqueEmail = false;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._";
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
var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Testing"))
{
    app.UseDeveloperExceptionPage();
    Console.WriteLine($"Using Developer Exception Page in {app.Environment.EnvironmentName} environment");
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseStatusCodePagesWithReExecute("/Error/HandleStatusCode/{0}");
    app.UseHsts();
    Console.WriteLine("Using Production error handling with HSTS");
}



app.UseStaticFiles();
app.UseSession();
app.UseRouting();

app.MapRazorPages();

app.UseAuthentication();
app.UseAuthorization();


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


// to check if the password validator is actually working
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    
    Console.WriteLine("Checking registered password validators...");
    var validators = serviceProvider.GetServices<IPasswordValidator<ApplicationUser>>().ToList();
    
    if (validators.Count == 0)
    {
        Console.WriteLine("⚠️ WARNING: No password validators are registered!");
    }
    else
    {
        Console.WriteLine($"Found {validators.Count} password validator(s):");
        foreach (var validator in validators)
        {
            Console.WriteLine($"✓ {validator.GetType().FullName}");
        }
        
        if (validators.Any(v => v.GetType().Name.Contains("CustomPasswordValidator")))
        {
            Console.WriteLine("✅ SUCCESS: CustomPasswordValidator is properly registered!");
        }
        else
        {
            Console.WriteLine("❌ ERROR: CustomPasswordValidator is NOT registered properly!");
        }
    }
    
    var options = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<IdentityOptions>>().Value;
    Console.WriteLine("\nPassword options configuration:");
    Console.WriteLine($"RequireDigit: {options.Password.RequireDigit}");
    Console.WriteLine($"RequireLowercase: {options.Password.RequireLowercase}");
    Console.WriteLine($"RequireNonAlphanumeric: {options.Password.RequireNonAlphanumeric}");
    Console.WriteLine($"RequireUppercase: {options.Password.RequireUppercase}");
    Console.WriteLine($"RequiredLength: {options.Password.RequiredLength}");
}

app.Run();