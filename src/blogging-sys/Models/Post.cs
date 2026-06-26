using System.ComponentModel.DataAnnotations;

namespace BloggingSystem.Models;

/// <summary>
/// Mirrors the Laravel <c>posts</c> table. A post belongs to (at most) one user.
/// The foreign key is nullable, matching the original migration.
/// </summary>
public class Post
{
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    // Nullable FK -> users.id (original column was nullable).
    public int? UserId { get; set; }
    public User? User { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
