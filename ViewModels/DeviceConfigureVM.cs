using System.ComponentModel;

namespace ParkingDemo.ViewModels
{
    public class DeviceConfigureVM
    {
        public int Id { get; set; }

        [DisplayName("Device Name")]
        public string DeviceName { get; set; }

        [DisplayName("Network Id")]
        public int Network { get; set; }

        [DisplayName("Node Id")]
        public int Node { get; set; }

        [DisplayName("Data Transmit Interval")]
        public int TransmitInterval { get; set; }

        [DisplayName("Wi-Fi SSID")]
        public string SSID { get; set; }

        [DisplayName("Wi-Fi Password")]
        public string Password { get; set; }
    }
}