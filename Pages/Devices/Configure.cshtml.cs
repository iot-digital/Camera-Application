using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ParkingDemo.Data;
using ParkingDemo.DTOs;
using ParkingDemo.Services;
using ParkingDemo.ViewModels;

namespace ParkingDemo.Pages.Devices
{
    [Authorize(Roles = "ADMIN")]
    public class ConfigureModel : PageModel
    {
        private readonly AppRepository _repository;

        [BindProperty]
        public DeviceConfigureVM ViewModel { get; set; } = new();

        public ConfigureModel(AppRepository repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id is null)
                return RedirectToPage("./Index");

            var device = await _repository.GetDeviceByIdAsync(id.Value);

            if (device == null)
                return RedirectToPage("./Index");

            ViewModel.DeviceName = device.Name;
            ViewModel.Network = device.Network;
            ViewModel.Node = device.Node;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || ViewModel == null)
            {
                return Page();
            }

            string msgContent = $"c:{ViewModel.Network},{ViewModel.Node},{ViewModel.TransmitInterval},{ViewModel.SSID},{ViewModel.Password}";

            Message configData = new Message()
            {
                Network = ViewModel.Network,
                Node = ViewModel.Node,
                Id = 1,
                Content = msgContent,
            };


            return RedirectToPage("./Index");
        }
    }
}
