using ParkingDemo.Models;
using System.Text.Json;

namespace ParkingDemo.Utilities
{
    public class HelperFunctions
    {
        public static Dictionary<string, int> ToDictionary(ZonesData obj)
        {
            var json = JsonSerializer.Serialize(obj);
            var dictionary = JsonSerializer.Deserialize<Dictionary<string, int>>(json);
            return dictionary;
        }
    }
}