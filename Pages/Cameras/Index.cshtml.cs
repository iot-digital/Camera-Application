using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ParkingDemo.Data;
using ParkingDemo.Models;
using ParkingDemo.Utilities;

namespace ParkingDemo.Pages.Cameras;

//[Authorize(Roles = "ADMIN,USER")]
public class IndexModel : PageModel
{
    private readonly AppRepository _repository;

    public IEnumerable<Camera>? Cameras { get; set; }

    public IndexModel(AppRepository repository)
    {
        _repository = repository;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        Cameras = await _repository.GetCamerasAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var result = await _repository.DeleteCameraFromDBAsync(id);

        Notify.Add(TempData, result, "Camera deleted successfully", "Couldn't delete the Camera. Try again.");

        Cameras = await _repository.GetCamerasAsync();

        return Page();
    }
}
