namespace ParkingDemo.Models;

public class Device
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Location { get; set; }
    public int Network { get; set; }
    public int Node { get; set; }
    public string? LastValue { get; set; }
    public DateTime? LastAlivePing { get; set; }
    public PowerInputType PowerInputType { get; set; }
    public int PowerState { get; set; }
    public string? Configuration { get; set; }

    public List<Camera>? Cameras { get; set; }

    public string Address => $"{Network}.{Node}";
}