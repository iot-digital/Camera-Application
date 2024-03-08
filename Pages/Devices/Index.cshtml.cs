using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ParkingDemo.Data;
using ParkingDemo.Models;
using ParkingDemo.Utilities;

namespace ParkingDemo.Pages.Devices;

//[Authorize(Roles = "ADMIN,USER")]
public class IndexModel : PageModel
{
    private readonly AppRepository _repository;

    public IEnumerable<Device>? Devices { get; set; }
    public List<Camera> Cameras { get; set; }

    public IndexModel(AppRepository repository)
    {
        _repository = repository;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        Devices = await _repository.GetDevicesAsync();

        return Page();
    }

  
    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
      
        var result = await _repository.DeleteDeviceFromDBAsync(id);

      
        Notify.Add(TempData, result, "Device and associated cameras deleted successfully", "Couldn't delete the device and associated cameras. Try again.");

 
        Cameras = await _repository.GetCamerasAsync();
        Devices = await _repository.GetDevicesAsync();

    
        return Page();
    }

}
