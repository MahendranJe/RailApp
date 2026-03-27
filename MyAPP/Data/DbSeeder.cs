using Microsoft.AspNetCore.Identity;
using MyAPP.Models;

namespace MyAPP.Data
{
    public static class DbSeeder
    {
        public static async Task SeedDataAsync(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            // Seed Roles
            string[] roles = { "Admin", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Seed Admin User (MahiJeya@18)
            var adminUsername = "MahiJeya@18";
            var adminUser = await userManager.FindByNameAsync(adminUsername);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminUsername,
                    Email = "mahijeya@gmail.com",
                    FullName = "Mahi Jeya",
                    Gender = "Male",
                    State = "Tamil Nadu",
                    City = "Chennai",
                    PhoneNumber = "9876543210",
                    MobileNumber = "9876543210",
                    EmailConfirmed = true,
                    IsActive = true,
                    IsVerified = true,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(adminUser, "Mahi@Jeya@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");

                    // Add premium subscription for admin
                    context.Subscriptions.Add(new Subscription
                    {
                        UserId = adminUser.Id,
                        PlanName = "Premium",
                        Price = 0,
                        StartDate = DateTime.UtcNow,
                        ExpiryDate = DateTime.UtcNow.AddYears(10),
                        IsActive = true
                    });
                    await context.SaveChangesAsync();
                }
            }

            // Seed Normal User (Rila@123)
            var normalUsername = "Rila@123";
            var normalUser = await userManager.FindByNameAsync(normalUsername);
            if (normalUser == null)
            {
                normalUser = new ApplicationUser
                {
                    UserName = normalUsername,
                    Email = "rila@gmail.com",
                    FullName = "Rila User",
                    Gender = "Female",
                    State = "Karnataka",
                    City = "Bangalore",
                    PhoneNumber = "9876543211",
                    MobileNumber = "9876543211",
                    EmailConfirmed = true,
                    IsActive = true,
                    IsVerified = true,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(normalUser, "Ril@#123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(normalUser, "User");

                    // Add free subscription for normal user
                    context.Subscriptions.Add(new Subscription
                    {
                        UserId = normalUser.Id,
                        PlanName = "Free",
                        Price = 0,
                        StartDate = DateTime.UtcNow,
                        IsActive = true
                    });
                    await context.SaveChangesAsync();

                    // Create welcome notification
                    context.Notifications.Add(new Notification
                    {
                        UserId = normalUser.Id,
                        Title = "Welcome to Train Updates!",
                        Message = "Thank you for registering. Subscribe to premium to access all train updates.",
                        CreatedAt = DateTime.UtcNow
                    });
                    await context.SaveChangesAsync();
                }
            }

            // Seed Train Updates
            if (!context.TrainUpdates.Any())
            {
                var admin = await userManager.FindByNameAsync(adminUsername);
                if (admin != null)
                {
                    var trainUpdates = new List<TrainUpdate>
                    {
                        new TrainUpdate
                        {
                            Title = "Rajdhani Express Schedule Update",
                            Description = "12301 Rajdhani Express from New Delhi to Howrah has been rescheduled. The train will now depart at 4:55 PM instead of 4:30 PM. Please check the updated timetable for more details. This change is effective from tomorrow.",
                            TrainNumber = "12301",
                            FromStation = "New Delhi",
                            ToStation = "Howrah",
                            TravelDate = DateTime.Today.AddDays(1),
                            IsPremium = false,
                            CreatedByUserId = admin.Id,
                            CreatedAt = DateTime.UtcNow.AddHours(-5)
                        },
                        new TrainUpdate
                        {
                            Title = "Shatabdi Express Premium Timetable",
                            Description = "Complete detailed timetable for 12002 Shatabdi Express including all stopping stations, arrival and departure times, platform numbers, and catering schedule. This premium content includes the official railway timetable image.",
                            TrainNumber = "12002",
                            FromStation = "New Delhi",
                            ToStation = "Bhopal",
                            TravelDate = DateTime.Today,
                            IsPremium = true,
                            ImagePath = "/images/sample-timetable.jpg",
                            CreatedByUserId = admin.Id,
                            CreatedAt = DateTime.UtcNow.AddHours(-3)
                        },
                        new TrainUpdate
                        {
                            Title = "Mumbai Duronto Cancellation Notice",
                            Description = "12261 Mumbai Duronto Express scheduled for tomorrow has been cancelled due to track maintenance work between Nagpur and Bhusawal. Passengers can claim full refund or reschedule their journey. Alternative trains are available.",
                            TrainNumber = "12261",
                            FromStation = "Mumbai CST",
                            ToStation = "Howrah",
                            TravelDate = DateTime.Today.AddDays(1),
                            IsPremium = false,
                            CreatedByUserId = admin.Id,
                            CreatedAt = DateTime.UtcNow.AddHours(-2)
                        },
                        new TrainUpdate
                        {
                            Title = "Mumbai Rajdhani - Premium Route Map & Timetable",
                            Description = "Exclusive detailed route map and complete timetable for 12951 Mumbai Rajdhani Express. Includes all intermediate stations, halt duration, distance from origin, and expected arrival times. Premium subscribers get access to downloadable PDF.",
                            TrainNumber = "12951",
                            FromStation = "Mumbai Central",
                            ToStation = "New Delhi",
                            TravelDate = DateTime.Today.AddDays(2),
                            IsPremium = true,
                            ImagePath = "/images/sample-timetable.jpg",
                            CreatedByUserId = admin.Id,
                            CreatedAt = DateTime.UtcNow.AddHours(-1)
                        },
                        new TrainUpdate
                        {
                            Title = "Karnataka Express Platform Change",
                            Description = "12627 Karnataka Express will now depart from Platform 5 instead of Platform 3 at New Delhi station. Please reach the station early and check the display boards for confirmation.",
                            TrainNumber = "12627",
                            FromStation = "New Delhi",
                            ToStation = "Bengaluru",
                            TravelDate = DateTime.Today,
                            IsPremium = false,
                            CreatedByUserId = admin.Id,
                            CreatedAt = DateTime.UtcNow.AddMinutes(-30)
                        },
                        new TrainUpdate
                        {
                            Title = "Purushottam Express - Complete Journey Guide",
                            Description = "Premium comprehensive guide for 12802 Purushottam Express journey. Includes detailed station-wise timetable, food availability, charging points, coach position, and tourist attractions along the route. Perfect for first-time travelers.",
                            TrainNumber = "12802",
                            FromStation = "Puri",
                            ToStation = "New Delhi",
                            TravelDate = DateTime.Today.AddDays(3),
                            IsPremium = true,
                            ImagePath = "/images/sample-timetable.jpg",
                            CreatedByUserId = admin.Id,
                            CreatedAt = DateTime.UtcNow
                        }
                    };

                    context.TrainUpdates.AddRange(trainUpdates);
                    await context.SaveChangesAsync();
                }
            }

            // Seed sample alerts for normal user
            if (!context.Alerts.Any())
            {
                var user = await userManager.FindByNameAsync(normalUsername);
                if (user != null)
                {
                    var alerts = new List<Alert>
                    {
                        new Alert
                        {
                            UserId = user.Id,
                            TrainNumber = "12301",
                            TravelDate = DateTime.Today.AddDays(3),
                            FromStation = "New Delhi",
                            ToStation = "Howrah",
                            CreatedAt = DateTime.UtcNow.AddDays(-1)
                        },
                        new Alert
                        {
                            UserId = user.Id,
                            TrainNumber = "12951",
                            TravelDate = DateTime.Today.AddDays(5),
                            FromStation = "Mumbai Central",
                            ToStation = "New Delhi",
                            CreatedAt = DateTime.UtcNow
                        }
                    };

                    context.Alerts.AddRange(alerts);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
