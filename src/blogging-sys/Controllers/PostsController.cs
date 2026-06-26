using BloggingSystem.Data;
using BloggingSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BloggingSystem.Controllers;

public class PostsController : Controller
{
    private const int PageSize = 10;
    private readonly BlogDbContext _db;

    public PostsController(BlogDbContext db)
    {
        _db = db;
    }

    // GET /Posts  -> list with optional title search + pagination.
    public async Task<IActionResult> Index(string? search, int page = 1)
    {
        if (page < 1) page = 1;

        var query = _db.Posts.Include(p => p.User).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.Title.Contains(search));

        var totalCount = await query.CountAsync();
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)PageSize));
        if (page > totalPages) page = totalPages;

        var posts = await query
            .OrderByDescending(p => p.Id)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        var model = new PostListViewModel
        {
            Posts = posts,
            Search = search,
            Page = page,
            TotalPages = totalPages,
            TotalCount = totalCount,
        };

        return View(model);
    }

    // GET /Posts/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var post = await _db.Posts.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == id);
        if (post is null)
            return NotFound();

        return View(post);
    }

    // GET /Posts/Create
    [Authorize]
    public IActionResult Create()
    {
        return View(new PostFormViewModel());
    }

    // POST /Posts/Create
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PostFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdStr == null || !int.TryParse(userIdStr, out int userId))
        {
            return Unauthorized();
        }

        _db.Posts.Add(new Post
        {
            Title = model.Title,
            Description = model.Description,
            UserId = userId,
        });
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET /Posts/Edit/5
    [Authorize]
    public async Task<IActionResult> Edit(int id)
    {
        var post = await _db.Posts.FindAsync(id);
        if (post is null)
            return NotFound();

        var currentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserIdStr == null || post.UserId?.ToString() != currentUserIdStr)
        {
            return Forbid();
        }

        var model = new PostFormViewModel
        {
            Id = post.Id,
            Title = post.Title,
            Description = post.Description,
        };

        return View(model);
    }

    // POST /Posts/Edit/5
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PostFormViewModel model)
    {
        var post = await _db.Posts.FindAsync(id);
        if (post is null)
            return NotFound();

        var currentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserIdStr == null || post.UserId?.ToString() != currentUserIdStr)
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            model.Id = id;
            return View(model);
        }

        post.Title = model.Title;
        post.Description = model.Description;
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // POST /Posts/Delete/5
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var post = await _db.Posts.FindAsync(id);
        if (post is null)
            return NotFound();

        var currentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserIdStr == null || post.UserId?.ToString() != currentUserIdStr)
        {
            return Forbid();
        }

        _db.Posts.Remove(post);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
