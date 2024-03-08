using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ParkingDemo.Models;
using ParkingDemo.Utilities;
using ParkingDemo.ViewModels;
using System.Data;

namespace ParkingDemo.Pages.Roles;

[Authorize(Roles = "ADMIN")]
public class CreateModel : PageModel
{
    private readonly RoleManager<AppRole> _roleManager;

    [BindProperty]
    public RoleVM ViewModel { get; set; } = new();

    public CreateModel(RoleManager<AppRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        AppRole appRole = new()
        {
            Name = ViewModel.Name
        };

        var result = await _roleManager.CreateAsync(appRole);
        if (!result.Succeeded)
        {
            ModelState.AddModelError("Name",
                string.Join(", ", result.Errors.Select(e => e.Description)));

            return Page();
        }

        Notify.Add(TempData, true, $"{ViewModel.Name} added successfully", "");

        return RedirectToPage("./Index");
    }
}
