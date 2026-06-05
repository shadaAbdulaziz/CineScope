using CineScope.Models;
using Microsoft.AspNetCore.Identity;

namespace CineScope.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            // Get required services
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var context = serviceProvider.GetRequiredService<AppDbContext>();

            // Create roles
            string[] roles = { "Admin", "Member" };

            foreach (var role in roles)
            {
                var roleExists = await roleManager.RoleExistsAsync(role);

                if (!roleExists)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Create default Admin user
            string adminEmail = "admin@cinescope.com";
            string adminPassword = "Admin@12345";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var user = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }

            // Seed sample movies only if the Movies table is empty
            if (!context.Movies.Any())
            {
                var movies = new List<Movie>
                {
                    new Movie
                    {
                        Title = "Echoes of Tomorrow",
                        Genre = "Sci-Fi, Drama",
                        ReleaseYear = 2026,
                        Rating = 8.4,
                        Duration = 128,
                        PosterUrl = "/images/posters/no-poster.png",
                        Description = "A scientist discovers a signal from the future that could change the fate of humanity."
                    },
                    new Movie
                    {
                        Title = "The Last Ember",
                        Genre = "Action, Adventure",
                        ReleaseYear = 2025,
                        Rating = 7.9,
                        Duration = 115,
                        PosterUrl = "/images/posters/no2-poster.png",
                        Description = "In a collapsing world, a young survivor protects the last source of energy from powerful enemies."
                    },
                    new Movie
                    {
                        Title = "Midnight Signal",
                        Genre = "Mystery, Thriller",
                        ReleaseYear = 2024,
                        Rating = 8.1,
                        Duration = 102,
                        PosterUrl = "/images/posters/no1-poster.png",
                        Description = "A late-night radio host receives a mysterious message that leads to a hidden city-wide conspiracy."
                    }
                };

                context.Movies.AddRange(movies);
                await context.SaveChangesAsync();
            }
        }
    }
}