using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ParkingDemo.Data;

namespace ParkingDemo.Pages
{
    public class IndexModel : PageModel
    {
        private readonly AppRepository _repository;
        public int DeviceCount { get; set; }
        public int CameraCount { get; set; }
        public int UserCount { get; set; }

        public IndexModel(AppRepository repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> OnGetAsync(int? lot = 0)
        {
                DeviceCount = await _repository.GetDevicesCountAsync();
                CameraCount = await _repository.GetCamerasCountAsync();
                UserCount = await _repository.GetUsersCountAsync();

                return Page();
        }
    }
}