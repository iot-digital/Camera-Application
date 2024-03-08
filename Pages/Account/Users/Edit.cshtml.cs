using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ParkingDemo.Data;
using ParkingDemo.Models;
using ParkingDemo.Utilities;
using ParkingDemo.ViewModels;

namespace ParkingDemo.Pages.Account.Users;

[Authorize(Roles = "ADMIN")]
public class EditModel : PageModel
{
    private readonly UserManager<AppUser> _userManager;
    private readonly AppRepository _repository;

    [BindProperty]
    public AppUserVMEdit ViewModel { get; set; } = new();

    public EditModel(
        UserManager<AppUser> userManager,
        AppRepository repository)
    {
        _userManager = userManager;
        _repository = repository;
    }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null || id == 1)
        {
            return NotFound();
        }

        var appUser = await _userManager.Users
            .FirstOrDefaultAsync(m => m.Id == id);

        if (appUser == null)
        {
            return NotFound();
        }

        ViewModel.Id = appUser.Id;
        ViewModel.FirstName = appUser.FirstName;
        ViewModel.LastName = appUser.LastName;
        ViewModel.Username = appUser.UserName;
        ViewModel.Email = appUser.Email;
        ViewModel.PhoneNumber = appUser.PhoneNumber;

        List<UserSelectionVM> selectedRoles = new();
        var roles = await _repository.GetAppRolesAsync();

        selectedRoles = roles
            .Select(r => new UserSelectionVM(r.Id, r.Name, false))
            .ToList();

        for (int i = 0; i < selectedRoles.Count; i++)
        {
            var selectedRole = selectedRoles[i];
            if (await _userManager.IsInRoleAsync(appUser, selectedRole.Name))
                selectedRole.IsSelected = true;
        }

        ViewModel.SelectedRoles = selectedRoles;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var appUser = await _userManager.Users
            .FirstOrDefaultAsync(m => m.Id == ViewModel.Id);

        appUser.Id = ViewModel.Id;
        appUser.FirstName = ViewModel.FirstName;
        appUser.LastName = ViewModel.LastName;
        appUser.UserName = ViewModel.Username;
        appUser.Email = ViewModel.Email;
        appUser.PhoneNumber = ViewModel.PhoneNumber;

        var identityResult = await _userManager.UpdateAsync(appUser);
        if (!identityResult.Succeeded)
        {
            ModelState.AddModelError("ViewModel.Username", string.Join(", ",
                identityResult.Errors.Select(e => e.Description)));

            return Page();
        }

        foreach (var role in ViewModel.SelectedRoles)
        {
            if (await _userManager.IsInRoleAsync(appUser, role.Name) && !role.IsSelected)
            {
                var result = await _userManager.RemoveFromRoleAsync(appUser, role.Name);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("ViewModel.SelectedRoles", $"The Role: {role.Name} could not be Removed.");
                }
            }
            else if (!await _userManager.IsInRoleAsync(appUser, role.Name) && role.IsSelected)
            {
                var result = await _userManager.AddToRoleAsync(appUser, role.Name);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("ViewModel.SelectedRoles", $"The Role: {role.Name} could not be Added.");
                }
            }
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        _repository.ResetCachedAppUsers();

        Notify.Add(TempData, true, $"User '{appUser.FirstName} {appUser.LastName}' updated successfully", "");

        return RedirectToPage("./Index");
    }
}
