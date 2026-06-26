using System.Security.Claims;
using BloggingSystem.Data;
using BloggingSystem.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BloggingSystem.Controllers;

public class AuthController : Controller
{
    private readonly BlogDbContext _db;
    private readonly PasswordHasher<User> _passwordHasher;

    public AuthController(BlogDbContext db)
    {
        _db = db;
        _passwordHasher = new PasswordHasher<User>();
    }

    // GET /Auth/Register
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Posts");

        return View(new RegisterViewModel());
    }

    // POST /Auth/Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Posts");

        if (!ModelState.IsValid)
            return View(model);

        // Check if email already exists
        var emailExists = await _db.Users.AnyAsync(u => u.Email == model.Email);
        if (emailExists)
        {
            ModelState.AddModelError(nameof(RegisterViewModel.Email), "A user with this email address already exists.");
            return View(model);
        }

        // Create new user
        var user = new User
        {
            Name = model.Name,
            Email = model.Email
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        // Sign in user
        await SignInUser(user, isPersistent: false);

        return RedirectToAction("Index", "Posts");
    }

    // GET /Auth/Login
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Posts");

        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel());
    }

    // POST /Auth/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Posts");

        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
            return View(model);

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt. Please check your credentials.");
            return View(model);
        }

        var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt. Please check your credentials.");
            return View(model);
        }

        // Sign in user
        await SignInUser(user, model.RememberMe);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Posts");
    }

    // POST /Auth/Logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Posts");
    }

    private async Task SignInUser(User user, bool isPersistent)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Email, user.Email)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = isPersistent,
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
        };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);
    }
}
