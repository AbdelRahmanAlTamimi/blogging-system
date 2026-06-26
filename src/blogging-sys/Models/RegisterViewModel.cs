using System.ComponentModel.DataAnnotations;

namespace BloggingSystem.Models;

public class RegisterViewModel
{
    [Required(ErrorMessage = "The name field is required.")]
    [StringLength(100, ErrorMessage = "The name must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "The email field is required.")]
    [EmailAddress(ErrorMessage = "The email field is not a valid e-mail address.")]
    [StringLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "The password field is required.")]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
