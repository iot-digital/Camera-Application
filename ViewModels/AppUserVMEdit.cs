using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ParkingDemo.ViewModels
{
    public class AppUserVMEdit
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("First Name")]
        public string? FirstName { get; set; }

        [Required]
        [DisplayName("Last Name")]
        public string? LastName { get; set; }

        [Required]
        [DisplayName("User Name")]
        public string? Username { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [DisplayName("Email ID")]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [DisplayName("Phone Number")]
        [StringLength(15, MinimumLength = 10, ErrorMessage = "Enter valid phone number")]
        public string? PhoneNumber { get; set; }

        [DisplayName("User Roles")]
        public List<UserSelectionVM> SelectedRoles { get; set; } = new();
    }
}