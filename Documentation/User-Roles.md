# User Roles Documentation

CineScope uses ASP.NET Identity with role-based authorization to control access to different parts of the application.

The system includes three main user types:

- Guest
- Member
- Admin

---

## 1. Guest User

A Guest is a user who is not logged in.

### Guest Permissions

Guest users can:

- View the home page
- Browse movies
- Search movies by title
- Filter movies by genre
- View movie details

### Guest Restrictions

Guest users cannot:

- Add movies
- Edit movies
- Delete movies
- Import movies from TMDB
- Add movies to favorites
- View the My Favorites page

---

## 2. Member User

A Member is a registered and logged-in user.

### Member Permissions

Member users can:

- Browse movies
- Search movies
- Filter movies by genre
- View movie details
- Add movies to favorites
- Remove movies from favorites
- View their My Favorites page

### Member Restrictions

Member users cannot:

- Add movies
- Edit movies
- Delete movies
- Import movies from TMDB

---

## 3. Admin User

An Admin user has full movie management permissions.

### Admin Permissions

Admin users can:

- Add movies manually
- Edit movies
- Delete movies
- Upload movie poster images
- Import movies from TMDB
- Browse movies
- Search and filter movies
- View movie details
- Use favorites

---

## Default Admin Account

The project creates a default Admin account using Seed Data.

```text
Email: admin@cinescope.com
Password: Admin@12345
