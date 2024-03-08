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
public class EditModel : PageModel
{
    private readonly RoleManager<AppRole> _roleManager;

    [BindProperty]
    public RoleVM ViewModel { get; set; } = new();

    public EditModel(RoleManager<AppRole> roleManager)
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

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var appRole = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Id == ViewModel.Id);
        appRole.Name = ViewModel.Name;

        var result = await _roleManager.UpdateAsync(appRole);
        if (!result.Succeeded)
        {
            ModelState.AddModelError("Name",
                string.Join(", ", result.Errors.Select(e => e.Description)));

            return Page();
        }

        Notify.Add(TempData, true, $"{ViewModel.Name} updated successfully", "");

        return RedirectToPage("./Index");
    }
}
