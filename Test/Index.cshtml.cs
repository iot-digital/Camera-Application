using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ParkingDemo.Pages.Test
{
    public class IndexModel : PageModel
    {
        public string ImagePath { get; set; }

        public void OnGet()
        {
            ImagePath = "/img/imageLogs/1c1eaa57-f684-4c21-8344-16f017372d0a_Test-CameraDevice1_2.jpg";
        }
    }
}
