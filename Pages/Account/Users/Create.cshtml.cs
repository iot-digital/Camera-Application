using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ParkingDemo.Data;
using ParkingDemo.Models;
using ParkingDemo.Utilities;
using ParkingDemo.ViewModels;

namespace ParkingDemo.Pages.Account.Users;

//[Authorize(Roles = "ADMIN")]
public class CreateModel : PageModel
{
    private readonly UserManager<AppUser> _userManager;
    private readonly AppRepository _repository;

    [BindProperty]
    public AppUserVMCreate ViewModel { get; set; } = new();

    public CreateModel(UserManager<AppUser> userManager, AppRepository repository)
    {
        _userManager = userManager;
        _repository = repository;
    }

    public async Task<IActionResult> OnGet()
    {
        var roles = await _repository.GetAppRolesAsync();

        ViewModel.SelectedRoles = roles
            .Select(r => new UserSelectionVM(r.Id, r.Name, false))
            .ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        AppUser appUser = new()
        {
            FirstName = ViewModel.FirstName,
            LastName = ViewModel.LastName,
            UserName = ViewModel.Username,
            Email = ViewModel.Email,
            PhoneNumber = ViewModel.PhoneNumber
        };

        var identityResult = await _userManager.CreateAsync(appUser);
        if (identityResult.Succeeded)
        {
            var passwordResult = await _userManager.AddPasswordAsync(appUser, ViewModel.Password);
            if (!passwordResult.Succeeded)
            {
                ModelState.AddModelError("ViewModel.Password", string.Join(", ",
                    passwordResult.Errors.Select(e => e.Description)));

                await _userManager.DeleteAsync(appUser);

                return Page();
            }
        }
        else
        {
            ModelState.AddModelError("ViewModel.Username", string.Join(", ",
                identityResult.Errors.Select(e => e.Description)));

            return Page();
        }

        foreach (var role in ViewModel.SelectedRoles)
        {
            if (role.IsSelected)
            {
                await _userManager.AddToRoleAsync(appUser, role.Name);
            }
        }

        _repository.ResetCachedAppUsers();

        Notify.Add(TempData, true, $"'{appUser.FirstName} {appUser.LastName}' created successfully", "");

        return RedirectToPage("./Index");
    }
}
