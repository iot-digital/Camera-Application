using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ParkingDemo.Data;
using ParkingDemo.Models;

namespace ParkingDemo.Pages.Dashboard
{
    public class IndexModel : PageModel
    {
        private readonly AppRepository _repository;
        public int DevicesCount { get; set; }
        public IEnumerable<Device> Devices { get; set; }
        public List<Camera> Cameras { get; set; }

        public IndexModel(AppRepository repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Devices = await _repository.GetDevicesAsync();
            Cameras = await _repository.GetCamerasAsync();
            DevicesCount = Devices.Count();

            return Page();
        }

        public string GetDeviceAliveState(DateTime? timestamp)
        {
            if(timestamp is null)
            {
                return "OFFLINE";
            }

            DateTime currentTime = DateTime.UtcNow;
            TimeSpan timeDiff = (TimeSpan)(currentTime - timestamp);

            if (timeDiff.TotalSeconds > 60)
            {
                return "OFFLINE";
            }

            return "ONLINE";
        }
    }
}
