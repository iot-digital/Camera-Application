using System.ComponentModel.DataAnnotations.Schema;

namespace ParkingDemo.Models;

public class CameraLog
{
    public int Id { get; set; }
    public int DeviceId { get; set; }
    public int CameraId { get; set; }
    public string? ImagePath { get; set; }

    [Column(TypeName = "jsonb")]
    public Dictionary<string, int>? Data { get; set; }   // { "CameraIndex":1, "ZoneId":2,"PersonCount":"2","car":"0","bike":"3","truck":"0","misc":"6" }
    public DateTime Timestamp { get; set; }
}