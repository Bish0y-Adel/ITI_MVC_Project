using Microsoft.AspNetCore.Mvc;

using System.ComponentModel.DataAnnotations;

namespace MCV.ViewModels.Account
{
    public class RegisterVM
    {
        [MaxLength(150), Required, MinLength(3)]
        public string FullName { get; set; }

        [Required(ErrorMessage = "*Email is required"), StringLength(50, MinimumLength = 3)]
        [RegularExpression(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$")]
        //[Remote("CheckEmail", "Account", AdditionalFields = nameof(Id))]
        public String Email { get; set; }

        [Required, StringLength(50, MinimumLength = 8)]
        public string Password { get; set; }
        [Compare(nameof(Password))]
        public string Cpassword { get; set; }

    }
}
