using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ParkingDemo.Utilities
{
    public static class Notify
    {
        public static void Add(ITempDataDictionary TempData, bool result, string success, string error)
        {
            if (result)
            {
                if (string.IsNullOrEmpty(TempData["success"] as string))
                    TempData["success"] = success;
                else
                    TempData["success"] += ", " + success;
            }
            else
            {
                if (string.IsNullOrEmpty(TempData["error"] as string))
                    TempData["error"] = error;
                else
                    TempData["error"] += ", " + error;
            }
        }

        public static void Clear(ITempDataDictionary TempData)
        {
            TempData.Clear();
        }
    }
}