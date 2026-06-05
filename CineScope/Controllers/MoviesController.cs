using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using CineScope.Data;
using CineScope.Models;
using CineScope.Services;

namespace CineScope.Controllers
{
    public class MoviesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly TmdbService _tmdbService;

        public MoviesController(AppDbContext context, TmdbService tmdbService)
        {
            _context = context;
            _tmdbService = tmdbService;
        }

        // GET: Movies
        public async Task<IActionResult> Index(string searchString, string movieGenre)
        {
            // Start with all movies from the database
            var movies = from m in _context.Movies
                         select m;

            // If the user typed something in the search box,
            // filter movies by title
            if (!string.IsNullOrEmpty(searchString))
            {
                movies = movies.Where(m => m.Title.Contains(searchString));
            }

            // If the user selected a genre from the dropdown,
            // filter movies by genre
            if (!string.IsNullOrEmpty(movieGenre))
            {
                movies = movies.Where(m => m.Genre == movieGenre);
            }

            // Get all unique genres from the Movies table
            // This will be used to fill the dropdown list in the View
            ViewBag.Genres = await _context.Movies
                .Select(m => m.Genre)
                .Distinct()
                .ToListAsync();

            // Save the current search text so it stays visible after searching
            ViewBag.SearchString = searchString;

            // Save the selected genre so it stays selected after filtering
            ViewBag.MovieGenre = movieGenre;

            // Send the filtered movies list to the Index view
            return View(await movies.ToListAsync());
        }

        // GET: Movies/Import
        [Authorize(Roles = "Admin")]
        public IActionResult Import()
        {
            return View(new List<TmdbMovieResult>());
        }

        // POST: Movies/Import
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Import(string query)
        {
            // If the search box is empty, show a message
            if (string.IsNullOrWhiteSpace(query))
            {
                ViewBag.Message = "Please enter a movie title to search.";
                return View(new List<TmdbMovieResult>());
            }

            // Search movies from TMDB using the service
            var results = await _tmdbService.SearchMoviesAsync(query);

            // Keep the search text visible in the view
            ViewBag.SearchQuery = query;

            // Show message if no movies were found
            if (results == null || !results.Any())
            {
                ViewBag.Message = "No movies found. Try another title.";
                return View(new List<TmdbMovieResult>());
            }

            return View(results);
        }

        // POST: Movies/ImportMovie
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ImportMovie(TmdbMovieResult tmdbMovie)
        {
            // Convert release date to year
            int releaseYear = 0;

            if (!string.IsNullOrEmpty(tmdbMovie.ReleaseDate) && tmdbMovie.ReleaseDate.Length >= 4)
            {
                int.TryParse(tmdbMovie.ReleaseDate.Substring(0, 4), out releaseYear);
            }

            // Get full movie details from TMDB to get Genre and Duration
            var details = await _tmdbService.GetMovieDetailsAsync(tmdbMovie.TmdbId);

            // Convert genres list to a single string, for example: Action, Drama
            string genre = "Unknown";

            if (details != null && details.Genres.Any())
            {
                genre = string.Join(", ", details.Genres
                    .Where(g => !string.IsNullOrEmpty(g.Name))
                    .Select(g => g.Name));
            }

            // Get movie duration from details
            int duration = details?.Duration ?? 0;

            // Convert TMDB result to our local Movie model
            var movie = new Movie
            {
                Title = tmdbMovie.Title ?? "Unknown Title",
                Description = tmdbMovie.Overview ?? "No description available.",
                ReleaseYear = releaseYear,
                Rating = tmdbMovie.Rating,
                PosterUrl = _tmdbService.GetPosterUrl(tmdbMovie.PosterPath),
                Genre = genre,
                Duration = duration
            };

            // Save movie to local database
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            // Check if current logged-in user already added this movie to favorites
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                ViewBag.IsFavorite = await _context.Favorites
                    .AnyAsync(f => f.UserId == userId && f.MovieId == id);
            }
            else
            {
                ViewBag.IsFavorite = false;
            }

            return View(movie);
        }

        // GET: Movies/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Movie movie, IFormFile? posterFile)
        {
            // Remove PosterUrl validation because poster is uploaded as a file now
            ModelState.Remove("PosterUrl");

            // Save uploaded poster image
            var posterPath = await SavePosterFileAsync(posterFile);

            if (!string.IsNullOrEmpty(posterPath))
            {
                movie.PosterUrl = posterPath;
            }
            else
            {
                // Use default poster if user did not upload an image
                movie.PosterUrl = "/images/no-poster.png";
            }

            if (ModelState.IsValid)
            {
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(movie);
        }

        // GET: Movies/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Movie movie, IFormFile? posterFile)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            // Remove PosterUrl validation because poster is uploaded as a file now
            ModelState.Remove("PosterUrl");

            if (ModelState.IsValid)
            {
                try
                {
                    // If user uploaded a new poster, save it and update PosterUrl
                    var posterPath = await SavePosterFileAsync(posterFile);

                    if (!string.IsNullOrEmpty(posterPath))
                    {
                        movie.PosterUrl = posterPath;
                    }
                    else
                    {
                        // Keep old poster if no new image uploaded
                        var oldMovie = await _context.Movies
                            .AsNoTracking()
                            .FirstOrDefaultAsync(m => m.Id == id);

                        if (oldMovie != null)
                        {
                            movie.PosterUrl = oldMovie.PosterUrl;
                        }
                        else
                        {
                            movie.PosterUrl = "/images/no-poster.png";
                        }
                    }

                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(movie);
        }

        // GET: Movies/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie != null)
            {
                _context.Movies.Remove(movie);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        // GET: Movies/MyFavorites
        [Authorize]
        public async Task<IActionResult> MyFavorites()
        {
            // Get the current logged-in user id
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            // Get only the movies that this user added to favorites
            var favoriteMovies = await _context.Favorites
                .Where(f => f.UserId == userId)
                .Include(f => f.Movie)
                .OrderByDescending(f => f.CreatedAt)
                .Select(f => f.Movie)
                .ToListAsync();

            return View(favoriteMovies);
        }

        // POST: Movies/AddToFavorites/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AddToFavorites(int movieId)
        {
            // Get the current logged-in user id
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            // Check if this movie is already in the user's favorites
            var alreadyExists = await _context.Favorites
                .AnyAsync(f => f.UserId == userId && f.MovieId == movieId);

            // Add movie to favorites only if it is not already added
            if (!alreadyExists)
            {
                var favorite = new Favorite
                {
                    UserId = userId,
                    MovieId = movieId,
                    CreatedAt = DateTime.Now
                };

                _context.Favorites.Add(favorite);
                await _context.SaveChangesAsync();
            }

            // Return to the movie details page
            return RedirectToAction(nameof(Details), new { id = movieId });
        }

        // POST: Movies/RemoveFromFavorites/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> RemoveFromFavorites(int movieId)
        {
            // Get the current logged-in user id
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            // Find the favorite record for this user and movie
            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.MovieId == movieId);

            // Remove the movie from favorites if it exists
            if (favorite != null)
            {
                _context.Favorites.Remove(favorite);
                await _context.SaveChangesAsync();
            }

            // Return to the movie details page
            return RedirectToAction(nameof(Details), new { id = movieId });
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }

        private async Task<string?> SavePosterFileAsync(IFormFile? posterFile)
        {
            // If no file is uploaded, return null
            if (posterFile == null || posterFile.Length == 0)
            {
                return null;
            }

            // Allow images only
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(posterFile.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
            {
                return null;
            }

            // Create a unique file name to avoid overwriting files
            var fileName = $"{Guid.NewGuid()}{extension}";

            // Physical folder path: wwwroot/images/posters
            var folderPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "images",
                "posters"
            );

            // Create folder if it does not exist
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Full physical file path
            var filePath = Path.Combine(folderPath, fileName);

            // Save file to wwwroot/images/posters
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await posterFile.CopyToAsync(stream);
            }

            // Return relative path to store in database
            return $"/images/posters/{fileName}";
        }
    }
}