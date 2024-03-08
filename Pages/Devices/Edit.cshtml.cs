using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ParkingDemo.Data;
using ParkingDemo.Utilities;
using ParkingDemo.ViewModels;

namespace ParkingDemo.Pages.Devices;

[Authorize(Roles = "ADMIN")]
public class EditModel : PageModel
{
    private readonly AppRepository _repository;

    [BindProperty]
    public DeviceVMCreateEdit ViewModel { get; set; } = new();

    public EditModel(AppRepository repository)
    {
        _repository = repository;
    }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
            return RedirectToPage("./Index");

        var vm = await _repository.GetDeviceByIdAsync(id.Value);

        if (vm == null)
            return RedirectToPage("./Index");

        ViewModel.Id = vm.Id;
        ViewModel.Name = vm.Name;
        ViewModel.Location = vm.Location;
        ViewModel.Network = vm.Network;
        ViewModel.Node = vm.Node;
        ViewModel.PowerInputType = vm.PowerInputType;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid || ViewModel == null)
        {
            return Page();
        }

        var result = await _repository.UpdateDeviceAsync(ViewModel);

        Notify.Add(TempData, result, $"Device updated successfully", $"Couldn't update Device");

        return RedirectToPage("./Index");
    }
}
