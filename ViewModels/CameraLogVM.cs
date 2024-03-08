using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParkingDemo.ViewModels
{
    public class CameraLogVM
    {
        public int Id { get; set; }

        [DisplayName("Device Name")]
        public string DeviceName { get; set; }

        [DisplayName("Camera Name")]
        public string CameraName { get; set; }

        [DisplayName("Image")]
        public string ImagePath { get; set; }

        [DisplayName("Zone ID")]
        public int ZoneId { get; set; }

        [DisplayName("Car")]
        public int CarCount { get; set; }

        [DisplayName("Person")]
        public int PersonCount { get; set; }

        [DisplayName("Truck")]
        public int TruckCount { get; set; }

        [DisplayName("Bike")]
        public int BikeCount { get; set; }

        [DisplayName("Misc")]
        public int MiscCount { get; set; }

        [DisplayName("Timestamp")]
        public DateTime Timestamp { get; set; }
    }
}