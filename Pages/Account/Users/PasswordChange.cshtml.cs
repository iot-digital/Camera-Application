using ParkingDemo.Data;
using ParkingDemo.Models;
using ParkingDemo.Utilities;
using ParkingDemo.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace ParkingDemo.Pages.Account.Users
{
    [Authorize]
    public class PasswordChangeModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDBContext _context;

        [BindProperty]
        public AppUserPasswordChangeVM ViewModel { get; set; }

        public PasswordChangeModel(UserManager<AppUser> userManager, AppDBContext context)
        {
            _userManager = userManager;
            _context = context;
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

            var appUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            if(appUser == null)
            {
                ModelState.AddModelError("ViewModel.CurrentPassword", "User Not Found");
                Notify.Add(TempData, true, "", $"User Not Found. Try again later");

                return Page();
            }

            var identityResult = await _userManager.ChangePasswordAsync(appUser, ViewModel.CurrentPassword, ViewModel.NewPassword);

            if (!identityResult.Succeeded)
            {
                ModelState.AddModelError("ViewModel.CurrentPassword", string.Join(", ",
                    identityResult.Errors.Select(e => e.Description)));

                return Page();
            }

            Notify.Add(TempData, true, $"User ({appUser.FirstName} {appUser.LastName})'s password updated successfully", "");

            return RedirectToPage("/Index");
        }
    }
}
