# CineScope – Modern Movie Discovery Platform

CineScope is a modern movie discovery and management web application built with **ASP.NET Core MVC**, **SQL Server**, **Entity Framework Core**, and **ASP.NET Identity**.

The application allows users to browse movies, search and filter movies, view movie details, add movies to favorites, and allows Admin users to manage movies and import movie data from the TMDB API.

---

## Project Features

### Public Features

Guest users can:

* Browse movies
* Search movies by title
* Filter movies by genre
* View movie details

### Member Features

Logged-in users can:

* Browse and search movies
* View movie details
* Add movies to favorites
* Remove movies from favorites
* View their personal favorites list

### Admin Features

Admin users can:

* Add movies manually
* Edit movies
* Delete movies
* Upload movie poster images
* Import movies from TMDB API
* Manage movie records using full CRUD operations

---

## Tech Stack

* ASP.NET Core MVC
* C#
* SQL Server
* Entity Framework Core
* ASP.NET Identity
* Role-Based Authorization
* Razor Views
* Bootstrap 5
* HTML / CSS
* TMDB API

---

## User Roles

The project uses ASP.NET Identity with role-based authorization.

### Guest

Guest users can browse, search, filter, and view movie details.

### Member

Member users can use the Favorites feature.

### Admin

Admin users have full movie management access.

Admin-only features are protected using:


## Default Admin Account

The project automatically creates a default Admin account using Seed Data.

Use the following credentials to log in as Admin:

```text
Email: admin@cinescope.com
Password: Admin@12345
```

After logging in as Admin, the following buttons and pages become available:

* Add Movie
* Edit Movie
* Delete Movie
* Import Movie from TMDB

---

## Seed Data

The project includes Seed Data to make testing easier.

When the application starts, it automatically creates:

* Admin role
* Member role
* Default Admin user
* Sample movies if the Movies table is empty

This means that after running the database migrations and starting the application, the reviewer can immediately log in using the default Admin account and see sample movie data.

The Seed Data is located in:

```text
Data/SeedData.cs
```

The Seed Data is executed from:

```text
Program.cs
```


## Database Setup

The project uses SQL Server and Entity Framework Core Migrations.

The project includes tables for:

* Movies
* Favorites
* ASP.NET Identity users
* ASP.NET Identity roles




## TMDB API Configuration

CineScope uses the TMDB API to import movie metadata.

The TMDB API is used to fetch:

* Movie title
* Description
* Release year
* Rating
* Poster image
* Genre
* Duration

For security reasons, the real TMDB API key should not be committed to GitHub.

Before running the Import Movie feature, create your own TMDB API key :

add your key:

```json
"Tmdb": {
  "ApiKey": "PUT_YOUR_TMDB_API_KEY_HERE",
  "BaseUrl": "https://api.themoviedb.org/3",
  "ImageBaseUrl": "https://image.tmdb.org/t/p/w500"
}
```

Replace:

```text
PUT_YOUR_TMDB_API_KEY_HERE
```

with your own TMDB API key.

If the API key is not configured, the manual movie features will still work, but the TMDB Import feature will not return results.

---

## Main Pages

### Public Pages

```text
/
Home page
```

```text
/Movies
Movies list
```

```text
/Movies/Details/{id}
Movie details
```

### Identity Pages

```text
/Identity/Account/Register
Register
```

```text
/Identity/Account/Login
Login
```

### Member Page

```text
/Movies/MyFavorites
My Favorites
```

### Admin Pages

```text
/Movies/Create
Add Movie
```

```text
/Movies/Edit/{id}
Edit Movie
```

```text
/Movies/Delete/{id}
Delete Movie
```

```text
/Movies/Import
Import Movie from TMDB
```

---

## Movie Features

The Movie model includes:

```text
Id
Title
Genre
ReleaseYear
Rating
Duration
PosterUrl
Description
```

Admin users can add movies manually or import movies from TMDB.

Movie posters can be uploaded manually and stored under:

```text
wwwroot/images/posters
```

A default poster image can be stored at:

```text
wwwroot/images/no-poster.png
```

---

## Favorites Feature

The Favorites feature allows logged-in users to save movies to their personal list.

The Favorites table stores:

```text
Id
UserId
MovieId
CreatedAt
```

Each user has their own favorites list.

Favorites page:

```text
/Movies/MyFavorites
```

---

## API Integration

The TMDB API integration is implemented through:

```text
Services/TmdbService.cs
```

This service handles:

* Searching movies from TMDB
* Getting movie details by TMDB movie ID
* Building full poster image URLs


---

## Completed Project Levels

### Level 1 – Foundation

Completed:

* ASP.NET Core MVC project
* SQL Server database
* Entity Framework Core migrations
* Movie model
* CRUD operations
* Search by title

### Level 2 – Modern UI

Completed:

* Dark theme
* Responsive homepage
* Hero banner
* Movie cards
* Hover effects
* Genre filtering
* Styled CRUD pages
* Styled Login and Register pages

### Level 3 – Authentication & Roles

Completed:

* Register
* Login
* Logout
* Admin role
* Member role
* Role-based authorization
* Admin-only movie management

### Level 4 – Favorites

Completed:

* Add to Favorites
* Remove from Favorites
* My Favorites page
* User-specific favorites

### Level 5 – External API Integration

Completed:

* TMDB API setup
* Search movies from TMDB
* Import movies from TMDB
* Fetch poster, genre, duration, rating, and description

---

## Future Improvements

Possible future improvements:

* Movie reviews
* User ratings
* Average rating calculation
* Admin dashboard
* Top-rated movies
* Trending movies import
* Prevent duplicate movie imports
* Deployment to Azure
* GitHub Actions CI/CD
* SignalR real-time features

---

