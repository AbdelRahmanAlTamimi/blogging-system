using BloggingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BloggingSystem.Data;

/// <summary>
/// Seeds a handful of users and posts on first run so the app is usable
/// immediately. No-ops if data already exists.
/// </summary>
public static class DbSeeder
{
    public static void Seed(BlogDbContext db)
    {
        // If there are existing users without a password hash, update them
        var usersWithoutHash = db.Users.Where(u => string.IsNullOrEmpty(u.PasswordHash)).ToList();
        if (usersWithoutHash.Any())
        {
            var pHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();
            foreach (var user in usersWithoutHash)
            {
                user.PasswordHash = pHasher.HashPassword(user, "password123");
            }
            db.SaveChanges();
        }

        if (db.Users.Any() || db.Posts.Any())
            return;

        var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();
        var users = new List<User>
        {
            new() { Name = "Ada Lovelace", Email = "ada@example.com" },
            new() { Name = "Alan Turing", Email = "alan@example.com" },
            new() { Name = "Grace Hopper", Email = "grace@example.com" },
        };
        foreach (var user in users)
        {
            user.PasswordHash = hasher.HashPassword(user, "password123");
        }
        db.Users.AddRange(users);
        db.SaveChanges();

        var samples = new (string Title, string Description)[]
        {
            ("Welcome to Blogging System", "This ASP.NET Core MVC blogging system is rebuilt with Entity Framework Core and MySQL."),
            ("Getting started with ASP.NET Core MVC", "Controllers, Razor views and the model-binding pipeline make porting a Laravel CRUD app straightforward."),
            ("Entity Framework Core basics", "EF Core replaces Eloquent here: DbContext, DbSet, migrations and LINQ queries against MySQL via the Pomelo provider."),
            ("Server-side validation", "Data annotations on the view model enforce the same rules as Laravel's validator: title min 3, description min 5."),
            ("Pagination explained", "The post list pages results server-side with Skip/Take and a Bootstrap pager."),
            ("Searching posts by title", "A simple Contains filter narrows the list and the term is preserved across pages."),
            ("Bootstrap 5 styling", "The UI keeps the familiar Bootstrap 5 look from the original Blogging System."),
            ("Working with migrations", "dotnet ef migrations add and database update keep the schema in sync with the models."),
            ("Seeding sample data", "DbSeeder inserts users and posts on first run so the app isn't empty."),
            ("Deploying your blog", "Publish with dotnet publish and point the connection string at your production MySQL server."),
            ("Editing and deleting posts", "Full CRUD support lets you update or remove posts, just like the Laravel original."),
            ("Wrapping up", "That's the tour - explore the code to see how each piece maps back to the Laravel version."),
        };

        var rotation = 0;
        foreach (var (title, description) in samples)
        {
            db.Posts.Add(new Post
            {
                Title = title,
                Description = description,
                UserId = users[rotation % users.Count].Id,
            });
            rotation++;
        }
        db.SaveChanges();
    }
}
