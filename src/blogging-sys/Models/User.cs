using System.ComponentModel.DataAnnotations;

namespace BloggingSystem.Models;

/// <summary>
/// Mirrors the Laravel <c>users</c> table. A user can author many posts.
/// </summary>
public class User
{
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation: one user -> many posts.
    public ICollection<Post> Posts { get; set; } = new List<Post>();
}
