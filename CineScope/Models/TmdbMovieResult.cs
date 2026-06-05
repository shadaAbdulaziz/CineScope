using System.Text.Json.Serialization;

namespace CineScope.Models
{
    // This class represents the full response returned from TMDB search API
    public class TmdbSearchResponse
    {
        [JsonPropertyName("results")]
        public List<TmdbMovieResult> Results { get; set; } = new();
    }

    // This class represents one movie result from TMDB search
    public class TmdbMovieResult
    {
        [JsonPropertyName("id")]
        public int TmdbId { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("overview")]
        public string? Overview { get; set; }

        [JsonPropertyName("release_date")]
        public string? ReleaseDate { get; set; }

        [JsonPropertyName("poster_path")]
        public string? PosterPath { get; set; }

        [JsonPropertyName("vote_average")]
        public double Rating { get; set; }
    }

    // This class represents full movie details from TMDB
    public class TmdbMovieDetails
    {
        [JsonPropertyName("id")]
        public int TmdbId { get; set; }

        [JsonPropertyName("runtime")]
        public int Duration { get; set; }

        [JsonPropertyName("genres")]
        public List<TmdbGenre> Genres { get; set; } = new();
    }

    // This class represents one genre from TMDB
    public class TmdbGenre
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }
}