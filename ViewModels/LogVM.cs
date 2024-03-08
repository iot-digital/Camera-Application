using ParkingDemo.Models;
using System.ComponentModel;

namespace ParkingDemo.ViewModels;

public class LogVM
{
    public int Id { get; set; }

    [DisplayName("Log Type")]
    public LogType Type { get; set; }

    public string Parameters { get; set; } = string.Empty;

    [DisplayName("Device Name")]
    public string? DeviceName { get; set; }

    [DisplayName("Location")]
    public string? Location { get; set; }

    [DisplayName("Camera")]
    public string? Camera { get; set; }

    [DisplayName("Camera Image")]
    public string? Image { get; set; }

    [DisplayName("Date & Time")]
    public DateTime Timestamp { get; set; }
}