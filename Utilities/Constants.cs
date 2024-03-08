namespace ParkingDemo.Utilities;

public static class Constants
{
    #region Roles

    public const string ADMIN = "ADMIN";
    public const string USER = "USER";

    #endregion

    #region Cache

    public const int CACHE_REFRESH_PERIOD = 60;
    public const string CACHE_USERS = "users";
    public const string CACHE_ROLES = "roles";
    public const string CACHE_DEVICES = "devices";
    public const string CACHE_CAMERAS = "cameras";
    public const string CACHE_DASHBOARD = "dashboard";
    public const string CACHE_CAMERA_LOGS = "cam_logs";
    public const string CACHE_DEVICES_ALIVE = "alive_devices";

    #endregion

    #region Status

    public const int ALIVE_INTERVAL = 30;

    #endregion


    #region Camera Object Indicies

    public static readonly Dictionary<int, string> CAMERA_OBJECTS = new()
    {
        { 1, "Person" },
        { 2, "Car" },
        { 3, "Truck" },
        { 4, "Bike" },
        { 5, "Misc" },
    };

    #endregion

    #region Message Modes

    public const int OBJECT_DATA = 1;
    public const int HEARTBEAT_DATA = 2;
    public const int CONFIG_DATA = 3;
    public const int LIVE_STREAM_DATA = 4;

    #endregion
}
