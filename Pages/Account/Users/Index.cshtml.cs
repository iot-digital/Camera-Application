using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ParkingDemo.Data;
using ParkingDemo.Models;
using ParkingDemo.Utilities;

namespace ParkingDemo.Pages.Account.Users;

[Authorize(Roles = "ADMIN,USER")]
public class IndexModel : PageModel
{
    private readonly AppRepository _repository;
    private readonly AppDBContext _context;

    public List<AppUser> AppUsers { get; set; } = default!;
    
    public IndexModel(AppDBContext context, AppRepository repository)
    {
        _context = context;
        _repository = repository;
    }

    public async Task OnGetAsync()
    {
        AppUsers = await _repository.GetAppUsersAsync();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id == null || _context.Users == null)
        {
            return NotFound();
        }

        var appUser = await _context.Users.FindAsync(id);

        int result = 0;
        if (appUser != null)
        {
            _context.Users.Remove(appUser);
            result = await _context.SaveChangesAsync();
        }

        _repository.ResetCachedAppUsers();

        Notify.Add(TempData, result > 0,
            $"'{appUser.FirstName} {appUser.LastName}' deleted successfully",
            "Couldn't delete the User");

        return RedirectToPage("./Index");
    }
}
