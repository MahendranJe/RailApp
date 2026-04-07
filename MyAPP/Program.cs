using Microsoft.AspNetCore.Identity;
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyAPP.Data;
using MyAPP.Models;
using MyAPP.Services;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// DEBUG MODE - Force Development environment for IIS debugging
// REMOVE THIS IN PRODUCTION!
// ============================================================
#if DEBUG
builder.Environment.EnvironmentName = "Development";
#endif
// ============================================================

// Configure file upload size limits (30MB)
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 31457280; // 30MB
});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 31457280; // 30MB
});

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext - Using MySQL (Aiven)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
    builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(8, 0, 0))
));

// Add Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password settings - At least 6 characters, one uppercase, one digit
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    
    // User settings
    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789@_";
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.SlidingExpiration = true;
});

// Register Services
builder.Services.AddScoped<ITrainUpdateService, TrainUpdateService>();
builder.Services.AddScoped<IAlertService, AlertService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();

// PAYMENT FEATURE - Keep registered for Admin functionality
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

// Register Background Service
builder.Services.AddHostedService<AlertBackgroundService>();

var app = builder.Build();




// Seed Database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        logger.LogWarning("========== DATABASE MIGRATION STARTED ==========");
        var context = services.GetRequiredService<ApplicationDbContext>();
        
        logger.LogWarning("Ensuring database exists...");
        await context.Database.EnsureCreatedAsync();
        logger.LogWarning("? Database ensured");
        
        logger.LogWarning("Checking pending migrations...");
        var pendingMigrations = (await context.Database.GetPendingMigrationsAsync()).ToList();
        if (pendingMigrations.Count > 0)
        {
            logger.LogWarning($"Found {pendingMigrations.Count} pending migrations:");
            foreach (var migration in pendingMigrations)
            {
                logger.LogWarning($"  - {migration}");
            }
            
            logger.LogWarning("Applying migrations...");
            await context.Database.MigrateAsync();
            logger.LogWarning("? Migrations completed successfully");
        }
        else
        {
            logger.LogWarning("? No pending migrations");
        }
        
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        
        logger.LogWarning("Seeding data...");
        await DbSeeder.SeedDataAsync(context, userManager, roleManager);
        logger.LogWarning("? Database seeding completed successfully");
        logger.LogWarning("========== DATABASE MIGRATION COMPLETED ==========");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "? An error occurred while seeding the database.");
        logger.LogError($"Exception Type: {ex.GetType().Name}");
        logger.LogError($"Message: {ex.Message}");
        logger.LogError($"StackTrace: {ex.StackTrace}");
        if (ex.InnerException != null)
        {
            logger.LogError($"Inner Exception Type: {ex.InnerException.GetType().Name}");
            logger.LogError($"Inner Exception Message: {ex.InnerException.Message}");
            logger.LogError($"Inner Exception StackTrace: {ex.InnerException.StackTrace}");
        }
        // Don't re-throw - log and continue so we can see other errors
    }
}

// ============================================================
// EXCEPTION HANDLING - Modified for IIS debugging
// ============================================================

// OPTION 1: Always show Developer Exception Page (TEMPORARY - FOR DEBUGGING ONLY!)
// Uncomment the line below to force detailed errors regardless of environment
app.UseDeveloperExceptionPage();

// OPTION 2: Original behavior - Use this in production
// if (!app.Environment.IsDevelopment())
// {
//     app.UseExceptionHandler("/Home/Error");
//     app.UseHsts();
// }

// OPTION 3: Log environment for debugging
var logger2 = app.Services.GetRequiredService<ILogger<Program>>();
logger2.LogWarning("===========================================");
logger2.LogWarning($"Current Environment: {app.Environment.EnvironmentName}");
logger2.LogWarning($"Is Development: {app.Environment.IsDevelopment()}");
logger2.LogWarning($"Content Root: {app.Environment.ContentRootPath}");
logger2.LogWarning("===========================================");

// ============================================================

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
