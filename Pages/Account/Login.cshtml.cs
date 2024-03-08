using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ParkingDemo.Data;
using ParkingDemo.Models;
using ParkingDemo.ViewModels;

namespace ParkingDemo.Pages.Account;

public class LoginModel : PageModel
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly AppRepository _repository;

    [BindProperty]
    public LoginVM LoginVM { get; set; } = new();

    public LoginModel(SignInManager<AppUser> signInManager, AppRepository repository)
    {
        _signInManager = signInManager;
        _repository = repository;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var passwordResult = await _signInManager.PasswordSignInAsync(
            LoginVM.Username, LoginVM.Password, true, true);

        if (passwordResult.Succeeded)
        {
            if (LoginVM.ReturnUrl is null)
            {
                return RedirectToPage("/Index");
            }
            else
            {
                string returnUrl = LoginVM.ReturnUrl.Substring(11);
                var decodedUrl = System.Net.WebUtility.UrlDecode(returnUrl);

                if (decodedUrl == "/")
                    decodedUrl = "/Index";
                else if (!decodedUrl.Contains('?') && !decodedUrl.EndsWith("Index"))
                    decodedUrl += "/Index";

                return RedirectToPage(decodedUrl);
            }
        }

        ModelState.AddModelError("LoginVM.Password", "Incorrect User Credentials.");

        return Page();
    }
}
