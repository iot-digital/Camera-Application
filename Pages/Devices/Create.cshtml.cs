using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ParkingDemo.Data;
using ParkingDemo.Utilities;
using ParkingDemo.ViewModels;

namespace ParkingDemo.Pages.Devices;

[Authorize(Roles = "ADMIN")]
public class CreateModel : PageModel
{
    private readonly AppRepository _repository;

    [BindProperty]
    public DeviceVMCreateEdit ViewModel { get; set; } = new();

    public CreateModel(AppRepository repository)
    {
        _repository = repository;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid || ViewModel == null)
        {
            return Page();
        }

        var result = await _repository.AddDeviceAsync(ViewModel);

        Notify.Add(TempData, result > 0, $"Device added successfully", $"Couldn't add Device to database");

        return RedirectToPage("./Index");
    }
}
