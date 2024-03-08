using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ParkingDemo.Data;
using ParkingDemo.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ParkingDemo.Pages.CameraDashboard
{
    public class IndexModel : PageModel
    {
        private readonly AppRepository _repository;
        public List<Camera> Cameras { get; set; }
        public Device Device { get; set; }
        public List<ZoneObjectsDict>? CameraZoneValues { get; set; }

        public IndexModel(AppRepository repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id is null)
                return RedirectToPage("./Index");

            Device = await _repository.GetDeviceByIdAsync(id.Value);
            Cameras = await _repository.GetCamerasByDeviceIdAsync(id.Value);

            // TODO: If class is used error occurs as "Object reference not set to an instance of an object." -> Solve it
            ExtractLastValueOfDevice();            

            return Page();
        }
        public string GetDeviceAliveState(DateTime? timestamp)
        {
            if (timestamp is null)
            {
                return "";
            }

            DateTime currentTime = DateTime.UtcNow;
            TimeSpan timeDiff = currentTime - timestamp.Value;

            if (timeDiff.TotalSeconds > 60)
            {
                return "";
            }

            return "";
        }

        private void ExtractLastValueOfDevice()
        {
            var lastValue = Device.LastValue;
            if (lastValue != null && lastValue.Contains("|"))
            {
                var extractZonesData = lastValue.Split("|");

                foreach (var data in extractZonesData)
                {
                    var countsData = data.Split(",");
                    
                    //ZoneObjectsDict objectsData = new ZoneObjectsDict
                    //{
                    //    ZoneName = "Zone" + countsData[1],
                    //    Person = int.Parse(countsData[2]),
                    //    Car = int.Parse(countsData[3]),
                    //    Truck= int.Parse(countsData[4]),
                    //    Bike = int.Parse(countsData[5]),
                    //    Misc = int.Parse(countsData[6]),
                    //};

                    //CameraZoneValues.Add(objectsData);
                }
            }
            else
            {
                var countsData = lastValue.Split(",");

                //ZonesData objectsData = new ZonesData
                //{
                //    ZoneName = "Zone" + countsData[1],
                //    Person = int.Parse(countsData[2]),
                //    Car = int.Parse(countsData[3]),
                //    Truck = int.Parse(countsData[4]),
                //    Bike = int.Parse(countsData[5]),
                //    Misc = int.Parse(countsData[6]),
                //};

                //CameraZoneValues.Add(objectsData);

            }
        }
    }


    public class ZoneObjectsDict
    {
        public string ZoneName { get; set; }
        public int Person { get; set; }
        public int Car { get; set; }
        public int Truck { get; set; }
        public int Bike { get; set; }
        public int Misc { get; set; }
    }

}
