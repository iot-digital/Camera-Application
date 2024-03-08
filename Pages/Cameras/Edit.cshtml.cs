using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ParkingDemo.Data;
using ParkingDemo.Models;
using ParkingDemo.Utilities;
using ParkingDemo.ViewModels;
//using Zone = Parking.ViewModels.Zone;

namespace ParkingDemo.Pages.Cameras;

[Authorize(Roles = "ADMIN")]
public class EditModel : PageModel
{
    private readonly AppRepository _repository;

    [BindProperty]
    public CameraVMCreateEdit ViewModel { get; set; } = new();
    public IEnumerable<Device>? DevicesList { get; set; }

    public EditModel(AppRepository repository)
    {
        _repository = repository;
    }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        DevicesList = await _repository.GetDevicesAsync();

        if (id is null)
            return RedirectToPage("./Index");

        var vm = await _repository.GetCameraByIdAsync(id.Value);

        if (vm == null)
            return RedirectToPage("./Index");

        ViewModel.Id = vm.Id;
        ViewModel.Name = vm.Name;
        ViewModel.Resolution = vm.Resolution;
        ViewModel.Index = vm.Index;
        ViewModel.Zones = vm.Zones;
        ViewModel.DeviceId = vm.DeviceId;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid || ViewModel == null)
        {
            return Page();
        }

        var result = await _repository.UpdateCameraAsync(ViewModel);

        Notify.Add(TempData, result, $"Camera updated successfully", $"Couldn't update Camera");

        return RedirectToPage("./Index");
    }
}
