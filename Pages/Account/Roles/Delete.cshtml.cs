using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ParkingDemo.Models;
using ParkingDemo.Utilities;
using ParkingDemo.ViewModels;
using System.Data;

namespace ParkingDemo.Pages.Roles;

[Authorize(Roles = "ADMIN")]
public class DeleteModel : PageModel
{
    private readonly RoleManager<AppRole> _roleManager;

    [BindProperty]
    public RoleVM ViewModel { get; set; } = new();

    public DeleteModel(RoleManager<AppRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var appRole = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Id == id.Value);

        ViewModel.Id = appRole.Id;
        ViewModel.Name = appRole.Name;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var appRole = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Id == ViewModel.Id);

        var result = await _roleManager.DeleteAsync(appRole);
        if (!result.Succeeded)
        {
            ModelState.AddModelError("Name",
                string.Join(", ", result.Errors.Select(e => e.Description)));

            return Page();
        }

        Notify.Add(TempData, true, $"Deleted successfully", "");

        return RedirectToPage("./Index");
    }
}
