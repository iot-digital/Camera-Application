using ParkingDemo.Models;

namespace ParkingDemo.Utilities;

public static class Extensions
{
    public static void UpdatePowerState(this Device device, string content)
    {
        PowerInputType powerInputType;
        int powerState;

        if (int.TryParse(content, out int powerStatus))
        {
            if (powerStatus == 200)
            {
                powerInputType = PowerInputType.Plugged;
                powerState = powerStatus;
            }
            else if (powerStatus <= 100)
            {
                powerInputType = PowerInputType.Battery;
                powerState = powerStatus;
            }
            else
            {
                powerInputType = PowerInputType.Battery;
                powerState = 50;
            }
        }
        else
        {
            powerInputType = PowerInputType.Plugged;
            powerState = 200;
        }

        device.PowerInputType = powerInputType;
        device.PowerState = powerState;
    }
}