using System.ComponentModel.DataAnnotations.Schema;

namespace ParkingDemo.Models;

public class Camera
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int Index { get; set; }

    [Column(TypeName = "jsonb")]
    public List<Zone>? Zones { get; set; }

    public ResolutionType Resolution { get; set; }
    public int DeviceId { get; set; }

    public Device? Device { get; set; }
}


public class Zone
{
    public string Name { get; set; }
    public int X1 { get; set; }
    public int Y1 { get; set; }
    public int X2 { get; set; }
    public int Y2 { get; set; }
    public int X3 { get; set; }
    public int Y3 { get; set; }
    public int X4 { get; set; }
    public int Y4 { get; set; }

    public override string ToString()
    {
        return $"[{Name} => ({X1}, {Y1}), ({X2}, {Y2}), ({X3}, {Y3}), ({X4}, {Y4})]";
    }
}
