using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CineScope.Models
{
    public class Favorite
    {
        public int Id { get; set; }

        // The user who added the movie to favorites
        [Required]
        public string UserId { get; set; } = string.Empty;

        // The movie added to favorites
        [Required]
        public int MovieId { get; set; }

        // Navigation property to the Movie
        public Movie? Movie { get; set; }

        // Date when the movie was added to favorites
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}