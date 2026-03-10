using System.ComponentModel.DataAnnotations;

namespace MCV.ViewModels.Account
{
    public class LoginVM
    {
        [Required(ErrorMessage = "*Email is required")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}
