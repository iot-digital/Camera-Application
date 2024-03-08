using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ParkingDemo.Data;
using ParkingDemo.Models;
using ParkingDemo.Utilities;
using ParkingDemo.ViewModels;
//using Zone = Parking.ViewModels.Zone;

namespace ParkingDemo.Pages.Cameras;

[Authorize(Roles = "ADMIN")]
public class CreateModel : PageModel
{
    private readonly AppRepository _repository;

    [BindProperty]
    public CameraVMCreateEdit ViewModel { get; set; } = new();

    public IEnumerable<Device>? DevicesList { get; set; }

    public CreateModel(AppRepository repository)
    {
        _repository = repository;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        DevicesList = await _repository.GetDevicesAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid || ViewModel == null)
        {
            return Page();
        }

        foreach (var zone in ViewModel.Zones) 
        {
            var zoneName = zone.Name;
            var x1 = zone.X1;
            var y1 = zone.Y1;
            var x2 = zone.X2;
            var y2 = zone.Y2;
            var x3 = zone.X3;
            var y3 = zone.Y3;
            var x4 = zone.X4;
            var y4 = zone.Y4;
        }


        var result = await _repository.AddCameraAsync(ViewModel);

        Notify.Add(TempData, result > 0, $"Camera added successfully", $"Couldn't add camera to database");

        return RedirectToPage("./Index");
    }

}