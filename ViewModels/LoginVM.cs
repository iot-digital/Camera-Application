using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ParkingDemo.ViewModels
{
    public class LoginVM
    {
        [Required]
        [DisplayName("User Name")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "User Name should be a Minmum of 3 Letters")]
        public string Username { get; set; } = null!;

        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Password should be a Minmum of 3 Characters")]
        public string Password { get; set; } = null!;

        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }
}