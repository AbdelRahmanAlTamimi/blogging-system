using System.ComponentModel.DataAnnotations;

namespace BloggingSystem.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "The email field is required.")]
    [EmailAddress(ErrorMessage = "The email field is not a valid e-mail address.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "The password field is required.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Remember me?")]
    public bool RememberMe { get; set; }
}
