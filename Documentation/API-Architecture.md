# API Architecture Documentation

## 1. Overview

CineScope integrates with the TMDB API to allow Admin users to search for movies from an external movie database and import movie metadata into the local SQL Server database.

The API integration reduces manual data entry and makes the movie management process faster and more realistic. Instead of entering all movie information manually, the Admin can search for a movie, review the returned results, and import the selected movie into the CineScope database.

---

## 2. API Provider

The external API used in this project is the TMDB API, also known as The Movie Database.

TMDB provides movie metadata such as movie title, overview, release date, rating, poster path, genres, and runtime.

---

## 3. Purpose of the API Integration

The purpose of the API integration is to support importing movie data automatically from an external source.

The imported data includes:

- Title
- Description/overview
- Release year
- Rating
- Poster image
- Genre
- Duration

This feature is available only for Admin users because imported movies are saved into the local database and become part of the official movie collection in the application.

---

## 4. API Configuration

TMDB API settings are stored in the `appsettings.json` file. The real API key is not included in the GitHub repository for security reasons.

```json
"Tmdb": {
  "ApiKey": "PUT_YOUR_TMDB_API_KEY_HERE",
  "BaseUrl": "https://api.themoviedb.org/3",
  "ImageBaseUrl": "https://image.tmdb.org/t/p/w500"
}
```

Anyone who runs the project should replace `PUT_YOUR_TMDB_API_KEY_HERE` with their own TMDB API key before testing the Import Movie feature.

---

## 5. API Service

The API logic is separated into a service class:

```text
Services/TmdbService.cs
```

This service is responsible for communicating with TMDB and keeps the controller cleaner by following the separation of concerns principle.

The service handles:

- Sending HTTP requests to TMDB
- Searching movies by title
- Getting full movie details by TMDB movie ID
- Building full poster image URLs

---

## 6. Main API Methods

### 6.1 SearchMoviesAsync

This method searches for movies by title.

```csharp
SearchMoviesAsync(string query)
```

It sends a request to the TMDB search endpoint:

```text
/search/movie
```

The response includes a list of matching movie results. Each result contains basic information such as TMDB ID, title, overview, release date, poster path, and vote average.

---

### 6.2 GetMovieDetailsAsync

This method gets full details for a selected movie using its TMDB ID.

```csharp
GetMovieDetailsAsync(int tmdbId)
```

It sends a request to the TMDB movie details endpoint:

```text
/movie/{tmdbId}
```

This response includes additional data such as genres and runtime. These values are used to fill the local movie fields `Genre` and `Duration`.

---

### 6.3 GetPosterUrl

This method builds the full poster image URL. TMDB returns only a poster path, and the application combines it with the configured image base URL.

```text
https://image.tmdb.org/t/p/w500 + poster_path
```

---

## 7. API Flow

1. Admin opens the Import Movie page.
2. Admin searches for a movie title.
3. CineScope sends a request to the TMDB Search API.
4. TMDB returns matching movie results.
5. Results are displayed as movie cards.
6. Admin clicks Import Movie.
7. CineScope sends another request to the TMDB Details API.
8. Genre and duration are fetched.
9. TMDB data is mapped to the local Movie model.
10. Movie is saved in SQL Server.
11. Imported movie appears in the Movies page.

---

## 8. Import Page

The Import Movie page is available at:

```text
/Movies/Import
```

This page allows the Admin to enter a movie title, search TMDB, view search results, and import a selected movie.

The page is implemented in:

```text
Views/Movies/Import.cshtml
```

---

## 9. Controller Actions

The API integration is handled through `MoviesController`.

Important actions include:

| Action | Purpose |
|---|---|
| `Import()` | Displays the Import Movie page. |
| `Import(string query)` | Searches TMDB based on the movie title entered by the Admin. |
| `ImportMovie(TmdbMovieResult tmdbMovie)` | Imports the selected movie, gets full movie details, maps the data to the local Movie model, and saves it into the local database. |

---

## 10. Authorization

The API import feature is available only for Admin users.

The Import actions are protected using:

```csharp
[Authorize(Roles = "Admin")]
```

This means Guest users and Member users cannot access the Import page. Only Admin users can search and import movies from TMDB.

---

## 11. Data Mapping

When a movie is imported from TMDB, the external API data is mapped to the local Movie model.

| TMDB Data | Local Movie Field |
|---|---|
| `title` | `Title` |
| `overview` | `Description` |
| `release_date` | `ReleaseYear` |
| `vote_average` | `Rating` |
| `poster_path` | `PosterUrl` |
| `genres` | `Genre` |
| `runtime` | `Duration` |

---

## 12. Local Database Storage

Imported movies are stored in the local database table:

```text
Movies
```

After import, the movie behaves like any manually added movie. Admin users can edit, delete, and view their details. Other users can browse the movie, search for it, and add it to favorites.

---

## 13. Security Notes

The TMDB API key should not be committed to GitHub. The repository should contain only a placeholder value:

```text
PUT_YOUR_TMDB_API_KEY_HERE
```

The reviewer or developer should replace this placeholder with their own API key in `appsettings.json`.

