# Blogging System (ASP.NET Core MVC)

A simple CRUD blog built with **ASP.NET Core MVC** and **Entity Framework Core** (MySQL).
It is a rebuild of the original Laravel blogging project, mapping the same
Posts + Users data model and CRUD behavior onto the .NET stack, plus a few practical
enhancements (search, pagination, sample-data seeding).

## Features

- **Posts CRUD** — create, view, edit, delete posts.
- **Users** — each post belongs to a user (one-to-many, nullable FK).
- **Server-side validation** — validation rules: title min 3 chars, description min 5 chars, post creator required and must exist.
- **Search** — filter the post list by title.
- **Pagination** — 10 posts per page; the search term is preserved across pages.
- **Seeding** — sample users and posts are inserted on first run.
- **Bootstrap 5** UI.

## Tech stack

| Concern        | Technology                                   |
| -------------- | -------------------------------------------- |
| Framework      | ASP.NET Core MVC (.NET 9)                    |
| ORM            | Entity Framework Core 9                       |
| Database       | MySQL via `Pomelo.EntityFrameworkCore.MySql` |
| UI             | Razor views + Bootstrap 5                     |

## Project layout

```
src/blogging-sys/
├── Models/        User, Post, PostFormViewModel, PostListViewModel
├── Data/          BlogDbContext, DbSeeder
├── Controllers/   HomeController, PostsController, AuthController
├── Views/         Home/, Posts/, Auth/, Shared/
├── Migrations/    InitialCreate
└── Program.cs     DI, DbContext registration, startup migrate + seed
```

## Prerequisites

- [.NET SDK 9](https://dotnet.microsoft.com/download)
- A running **MySQL** (or MariaDB) server
- EF Core CLI tools (only needed to manage migrations):
  ```bash
  dotnet tool install --global dotnet-ef --version 9.0.0
  ```

## Configuration

Set the connection string. The default in `appsettings.json` is:

```
server=localhost;port=3306;database=blogging_system;user=root;password=
```

Override it without editing the file using user-secrets (recommended for local dev):

```bash
cd src/blogging-sys
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "server=localhost;port=3306;database=blogging_system;user=root;password=YOUR_PASSWORD"
```

or via an environment variable:

```bash
# PowerShell
$env:ConnectionStrings__DefaultConnection = "server=localhost;port=3306;database=blogging_system;user=root;password=YOUR_PASSWORD"
```

> The server version is pinned in `Program.cs` (`MySqlServerVersion 8.0.31`) so the EF
> design-time tooling doesn't need a live database. Adjust it to match your server if needed.

## Run

```bash
cd src/blogging-sys
dotnet run
```

On startup the app **applies migrations automatically** and seeds sample data if the
tables are empty. Then browse to the URL shown in the console.

To apply migrations manually instead:

```bash
dotnet ef database update
```

## Managing the schema

```bash
# add a migration after changing the models
dotnet ef migrations add <Name>

# apply to the database
dotnet ef database update
```
