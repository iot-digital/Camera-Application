using ParkingDemo.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ParkingDemo.ViewModels;

public class DeviceVMCreateEdit
{
    public int Id { get; set; }

    [Required]
    [DisplayName("Device Name")]
    [StringLength(50, MinimumLength = 4, ErrorMessage = "The Number of Letters must be greater than 4 and less than 50")]
    public string? Name { get; set; }

    [Required]
    [DisplayName("Device Location")]
    [StringLength(50, MinimumLength = 4, ErrorMessage = "The Number of Letters must be greater than 4 and less than 50")]
    public string? Location { get; set; }

    [DisplayName("Power Input Type")]
    public PowerInputType PowerInputType { get; set; }

    [Required]
    [DisplayName("Network Address")]
    [Range(0, 9999999, ErrorMessage = "Network Range should be between 0 to 9,999,999")]
    public int Network { get; set; }

    [Required]
    [DisplayName("Node Address")]
    [Range(0, 9999999, ErrorMessage = "Network Range should be between 0 to 9,999,999")]
    public int Node { get; set; }

    public string? Configuration { get; set; }
}