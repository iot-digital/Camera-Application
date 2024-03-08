using ParkingDemo.Models;

namespace ParkingDemo.DTOs;

public class DashboardDTO
{
    public List<Device> Devices { get; set; }
    public List<CameraLog> CameraLogs { get; set; }
}