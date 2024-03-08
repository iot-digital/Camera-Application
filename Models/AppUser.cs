using Microsoft.AspNetCore.Identity;

namespace ParkingDemo.Models;

public class AppUser : IdentityUser<int>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    public string? FullName
    {
        get
        {
            return $"{FirstName} {LastName}";
        }
    }
}