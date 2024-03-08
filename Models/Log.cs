namespace ParkingDemo.Models;

public class Log
{
    public int Id { get; set; }
    public int DeviceId { get; set; }
    public LogType Type { get; set; }
    public string Parameters { get; set; } = string.Empty;
    public bool IsAcknowledged { get; set; }
    public DateTime Timestamp { get; set; }
}