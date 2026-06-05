using CineScope.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CineScope.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Movies table
        public DbSet<Movie> Movies { get; set; }

        // Favorites table
        public DbSet<Favorite> Favorites { get; set; }
    }
}