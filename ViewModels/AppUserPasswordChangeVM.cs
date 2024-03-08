using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ParkingDemo.ViewModels
{
    public class AppUserPasswordChangeVM
    {
        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        [DataType(DataType.Password)]
        [DisplayName("Current Password")]
        public string? CurrentPassword { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        [DataType(DataType.Password)]
        [DisplayName("New Password")]
        public string? NewPassword { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        [DataType(DataType.Password)]
        [DisplayName("Repeat New Password")]
        [Compare(nameof(NewPassword), ErrorMessage = "'New Password' and 'Repeat New Password' doesn't match")]
        public string? RepeatNewPassword { get; set; }
    }
}