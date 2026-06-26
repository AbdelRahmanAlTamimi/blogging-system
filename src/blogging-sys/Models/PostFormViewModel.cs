using System.ComponentModel.DataAnnotations;

namespace BloggingSystem.Models;

/// <summary>
/// Backs the Create/Edit post forms. Validation rules mirror the original
/// Laravel controller: title (required, min 3), description (required, min 5),
/// post_creator (required, must exist in users).
/// </summary>
public class PostFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "The title field is required.")]
    [MinLength(3, ErrorMessage = "The title must be at least 3 characters.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "The description field is required.")]
    [MinLength(5, ErrorMessage = "The description must be at least 5 characters.")]
    public string Description { get; set; } = string.Empty;
}
