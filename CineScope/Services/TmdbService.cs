using CineScope.Models;
using System.Text.Json;

namespace CineScope.Services
{
    public class TmdbService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public TmdbService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        // Search movies from TMDB by movie title
        public async Task<List<TmdbMovieResult>> SearchMoviesAsync(string query)
        {
            var apiKey = _configuration["Tmdb:ApiKey"];
            var baseUrl = _configuration["Tmdb:BaseUrl"];

            var url = $"{baseUrl}/search/movie?api_key={apiKey}&query={Uri.EscapeDataString(query)}";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return new List<TmdbMovieResult>();
            }

            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<TmdbSearchResponse>(json);

            return result?.Results ?? new List<TmdbMovieResult>();
        }

        // Get full movie details from TMDB using TMDB movie id
        public async Task<TmdbMovieDetails?> GetMovieDetailsAsync(int tmdbId)
        {
            var apiKey = _configuration["Tmdb:ApiKey"];
            var baseUrl = _configuration["Tmdb:BaseUrl"];

            var url = $"{baseUrl}/movie/{tmdbId}?api_key={apiKey}";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();

            var details = JsonSerializer.Deserialize<TmdbMovieDetails>(json);

            return details;
        }

        // Build full poster URL from TMDB poster path
        public string GetPosterUrl(string? posterPath)
        {
            if (string.IsNullOrEmpty(posterPath))
            {
                return "/images/no-poster.png";
            }

            var imageBaseUrl = _configuration["Tmdb:ImageBaseUrl"];
            return $"{imageBaseUrl}{posterPath}";
        }
    }
}