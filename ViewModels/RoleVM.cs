using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ParkingDemo.ViewModels
{
    public class RoleVM
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Role Name")]
        public string? Name { get; set; }
    }
}