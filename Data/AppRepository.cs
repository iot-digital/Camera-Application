using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ParkingDemo.Data;
using ParkingDemo.DTOs;
using ParkingDemo.Models;
using ParkingDemo.Utilities;
using ParkingDemo.ViewModels;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace ParkingDemo.Data;

public class AppRepository
{
    private AppDBContext? _context;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IMemoryCache _cache;
    private readonly IWebHostEnvironment _hostingEnvironment;

    public AppRepository(IServiceScopeFactory serviceScopeFactory, IMemoryCache cache, IWebHostEnvironment hostingEnvironment)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _cache = cache;
        _hostingEnvironment = hostingEnvironment;
    }

    #region init
    private AppDBContext GetAppDbContext()
    {
        var scope = _serviceScopeFactory.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<AppDBContext>();

        return _context;
    }
    #endregion

    #region Devices

    public async ValueTask<IEnumerable<Device>> GetDevicesAsync()
    {
        var result = await GetCachedDevicesAsync();

        return result is null ? Array.Empty<Device>() : result;
    }
    public async Task<int> GetDevicesCountAsync()
    {
        var devices = await GetCachedDevicesAsync();
        return devices.Count();
    }


    public async ValueTask<Device?> GetDeviceByIdAsync(int id)
    {
        var devices = await GetCachedDevicesAsync();
        var device = devices.FirstOrDefault(e => e.Id == id);

        return device is null ? null : device;
    }

    public async Task<Device?> GetDeviceByAddressAsync(int network, int node)
    {
        var devices = await GetCachedDevicesAsync();
        var device = devices.FirstOrDefault(n => n.Network == network && n.Node == node);

        return device is null ? null : device;
    }

    public async Task<int> AddDeviceAsync(DeviceVMCreateEdit vm)
    {
        var ctx = GetAppDbContext();

        var device = new Device
        {
            Name = vm.Name,
            Location = vm.Location,
            Network = vm.Network,
            Node = vm.Node,
            PowerInputType = vm.PowerInputType,
            Configuration = string.Empty,
            LastValue = string.Empty,
            //LastAlivePing = DateTime.UtcNow.AddMinutes(-61),
            LastAlivePing = null,
            //LastAlivePing = DateTime.Now,
        };

        await ctx.Devices.AddAsync(device);

        var result = await ctx.SaveChangesAsync();
        if (result > 0)
        {
            ResetCachedDevices();
            return device.Id;
        }

        return 0;
    }

    public async Task<bool> UpdateDeviceAsync(DeviceVMCreateEdit vm)
    {
        var ctx = GetAppDbContext();

        var device = await ctx.Devices
            .FirstOrDefaultAsync(n => n.Id == vm.Id);

        if (device is null)
            return false;

        device.Name = vm.Name;
        device.Location = vm.Location;
        device.Network = vm.Network;
        device.Node = vm.Node;
        device.PowerInputType = vm.PowerInputType;
        device.Configuration = vm.Configuration;

        var result = await ctx.SaveChangesAsync();
        if (result > 0)
        {
            ResetCachedDevices();
            return true;
        }

        return false;
    }
    public async Task<bool> DeleteDeviceFromDBAsync(int Id)
    {
        var ctx = GetAppDbContext();

        var device = await ctx.Devices
            .Include(d => d.Cameras) // Include cameras associated with the device
            .FirstOrDefaultAsync(n => n.Id == Id);

        if (device is null)
            return false;

        // Delete cameras associated with the device
        ctx.Cameras.RemoveRange(device.Cameras);

        // Now remove the device itself
        ctx.Devices.Remove(device);

        var result = await ctx.SaveChangesAsync();
        if (result > 0)
        {
            ResetCachedDevices();
            ResetCachedCameras(); // Call the method to reset the camera cache
        }

        return result > 0;
    }



    public async Task<bool> UpdateDeviceAlivePingAsync(int network, int node, string? content)
    {
        if (string.IsNullOrEmpty(content))
            content = "200";

        var ctx = GetAppDbContext();

        var device = await ctx.Devices
            .FirstOrDefaultAsync(d => d.Network == network && d.Node == node);

        if (device is null)
            return false;

        device.LastAlivePing = DateTime.UtcNow;

        if (content.Contains('!'))
        {
            var split = content.Split('!', StringSplitOptions.RemoveEmptyEntries);

            device.UpdatePowerState(split[0]);
            device.LastValue = split[1];
        }
        else
        {
            device.UpdatePowerState(content);
        }

        var result = await ctx.SaveChangesAsync();
        if (result > 0)
        {
            var cachedDevices = await GetCachedDevicesAsync();
            Device? updatedDevice = null;
            for (int i = 0; i < cachedDevices.Count; i++)
            {
                if (cachedDevices[i].Id == device.Id)
                {
                    cachedDevices[i].LastAlivePing = device.LastAlivePing;
                    cachedDevices[i].LastValue = device.LastValue;
                    cachedDevices[i].PowerInputType = device.PowerInputType;
                    cachedDevices[i].PowerState = device.PowerState;

                    updatedDevice = cachedDevices[i];
                    break;
                }
            }
        }

        return result > 0;
    }

    public async Task<bool> UpdateDeviceValueAsync(MessageTrackItem messageTrackItem)
    {
        var ctx = GetAppDbContext();

        var device = await ctx.Devices
            .FirstOrDefaultAsync(d => d.Id == messageTrackItem.DeviceId);

        if (device is null)
            return false;

        device.LastValue = messageTrackItem.Message.ContentValue;

        var result = await ctx.SaveChangesAsync();
        if (result > 0)
        {
            var cachedDevices = await GetCachedDevicesAsync();
            for (int i = 0; i < cachedDevices.Count; i++)
            {
                if (cachedDevices[i].Id == device.Id)
                {
                    cachedDevices[i].LastValue = device.LastValue;
                    break;
                }
            }
        }

        return result > 0;
    }

    public async Task<bool> UpdateDeviceValueAsync(int network, int node, string content)
    {
        var ctx = GetAppDbContext();

        var device = await ctx.Devices
            .FirstOrDefaultAsync(d => d.Network == network && d.Node == node);

        if (device is null)
            return false;

        device.LastValue = content;

        var result = await ctx.SaveChangesAsync();
        if (result > 0)
        {
            var cachedDevices = await GetCachedDevicesAsync();
            for (int i = 0; i < cachedDevices.Count; i++)
            {
                if (cachedDevices[i].Id == device.Id)
                {
                    cachedDevices[i].LastValue = device.LastValue;
                    break;
                }
            }
        }

        return result > 0;
    }

    public async Task<(bool, int)> UpdateDeviceValueAndAddLogAsync(
        int network, int node, string value, LogType type = LogType.Information, bool isAcknowledged = true)
    {
        var ctx = GetAppDbContext();

        var device = await ctx.Devices
            .Include(e => e.Cameras)
            .FirstOrDefaultAsync(d => d.Network == network && d.Node == node);

        if (device is null || device.LastValue == value)
            return (false, 0);

        device.LastValue = value;
        device.LastAlivePing = DateTime.UtcNow;

        Log log = new()
        {
            DeviceId = device.Id,
            Type = type,
            Parameters = value,
            Timestamp = DateTime.UtcNow,
            IsAcknowledged = isAcknowledged,
        };

        await ctx.Logs.AddAsync(log);

        var result = await ctx.SaveChangesAsync();
        if (result > 0)
        {
            var cachedDevices = await GetCachedDevicesAsync();
            for (int i = 0; i < cachedDevices.Count; i++)
            {
                if (cachedDevices[i].Id == device.Id)
                {
                    cachedDevices[i].LastValue = device.LastValue;
                    cachedDevices[i].LastAlivePing = device.LastAlivePing;
                    break;
                }
            }
        }

        return (result > 0, log.Id);
    }

    public async Task<(bool, int, List<Dictionary<int, string>>)> UpdateDeviceValueAndAddCamLogAsync(
        int network, int node, string value, List<string>? images = null, LogType type = LogType.Information, bool isAcknowledged = true)
    {
        var ctx = GetAppDbContext();

        List<ZonesData> zonesData = ExtractZonesData(value);
        var camerasData = zonesData.DistinctBy(x => x.CameraIndex).ToList();

        var device = await ctx.Devices
            .Include(e => e.Cameras)
            .FirstOrDefaultAsync(d => d.Network == network && d.Node == node);

        if (device is null || device.LastValue == value)
            return (false, 0, new List<Dictionary<int, string>>());

        device.LastValue = value;
        device.LastAlivePing = DateTime.UtcNow;

        List<Dictionary<int, string>> imagePaths = new List<Dictionary<int, string>>();
        if (images != null)
        {
            int index = 0;

            var wwwrootPath = Path.Combine(_hostingEnvironment.WebRootPath, "img", "imageLogs");

            foreach (var image in images)
            {
                byte[] imageAsBytes = Convert.FromBase64String(image);

                var uniqueFileName = Guid.NewGuid().ToString() + ".jpg";
                var imagePathInWebRoot = Path.Combine(wwwrootPath, uniqueFileName);

                Directory.CreateDirectory(wwwrootPath);

                File.WriteAllBytes(imagePathInWebRoot, imageAsBytes);

                var imagePath = "/img/imageLogs/" + uniqueFileName;

                var imgDict = new Dictionary<int, string> { { camerasData[index].CameraIndex, imagePath } };
                imagePaths.Add(imgDict);

                index++;
            }
        }

        foreach (var data in zonesData)
        {
            var camLog = new CameraLog
            {
                DeviceId = device.Id,
                CameraId = device.Cameras.FirstOrDefault(c => c.Index == data.CameraIndex).Id,
                ImagePath = imagePaths
                                .SelectMany(dict => dict)
                                .Where(kvp => kvp.Key == data.CameraIndex)
                                .Select(kvp => kvp.Value)
                                .FirstOrDefault(),
                Data = HelperFunctions.ToDictionary(data),
                Timestamp = DateTime.UtcNow,
            };

            await ctx.CameraLogs.AddAsync(camLog);
        }

        Log log = new()
        {
            DeviceId = device.Id,
            Type = type,
            Parameters = value,
            Timestamp = DateTime.UtcNow,
            IsAcknowledged = isAcknowledged,
        };

        await ctx.Logs.AddAsync(log);

        var result = await ctx.SaveChangesAsync();
        if (result > 0)
        {
            var cachedDevices = await GetCachedDevicesAsync();
            for (int i = 0; i < cachedDevices.Count; i++)
            {
                if (cachedDevices[i].Id == device.Id)
                {
                    cachedDevices[i].LastValue = device.LastValue;
                    cachedDevices[i].LastAlivePing = device.LastAlivePing;
                    break;
                }
            }
        }

        return (result > 0, log.Id, imagePaths);
    }

    public async Task<(bool, int)> UpdateDeviceConfigAndAddLogAsync(
        int network, int node, string value, LogType type = LogType.Configuration, bool isAcknowledged = true)
    {
        var ctx = GetAppDbContext();

        var device = await ctx.Devices
            .FirstOrDefaultAsync(d => d.Network == network && d.Node == node);

        if (device is null)
            return (false, 0);

        device.Configuration = value;
        device.LastAlivePing = DateTime.UtcNow;

        Log log = new()
        {
            DeviceId = device.Id,
            Type = type,
            Parameters = value,
            Timestamp = DateTime.UtcNow,
            IsAcknowledged = isAcknowledged,
        };

        await ctx.Logs.AddAsync(log);

        var result = await ctx.SaveChangesAsync();
        if (result > 0)
        {
            var cachedDevices = await GetCachedDevicesAsync();
            for (int i = 0; i < cachedDevices.Count; i++)
            {
                if (cachedDevices[i].Id == device.Id)
                {
                    cachedDevices[i].Configuration = device.Configuration;
                    cachedDevices[i].LastAlivePing = device.LastAlivePing;
                    break;
                }
            }
        }

        return (result > 0, log.Id);
    }

    public async Task<(bool, int)> UpdateAckStatusAsync(MessageTrackItem messageTrackItem)
    {
        var ctx = GetAppDbContext();

        var log = await ctx.Logs
            .FirstOrDefaultAsync(n => n.Id == messageTrackItem.DbRowId);

        if (log is null)
            return (false, 0);

        log.IsAcknowledged = true;

        var result = await ctx.SaveChangesAsync();

        return (result > 0, messageTrackItem.DeviceId);
    }

    private List<ZonesData> ExtractZonesData(string value)
    {
        List<ZonesData> zonesDataList = new List<ZonesData>();

        var isMultipleZones = value.Contains("|");
        if (isMultipleZones)
        {
            var zonesData = value.Split('|');

            foreach (var data in zonesData)
            {
                var splitDataByComma = data.Split(",").Select(int.Parse).ToArray();

                var zoneData = new ZonesData
                {
                    CameraIndex = splitDataByComma[0],
                    ZoneId = splitDataByComma[1],
                    PersonCount = splitDataByComma[2],
                    CarCount = splitDataByComma[3],
                    TruckCount = splitDataByComma[4],
                    BikeCount = splitDataByComma[5],
                    MiscCount = splitDataByComma[6],
                };

                zonesDataList.Add(zoneData);
            }
        }
        else
        {
            var splitDataByComma = value.Split(",").Select(int.Parse).ToArray();
            var zoneData = new ZonesData
            {
                CameraIndex = splitDataByComma[0],
                ZoneId = splitDataByComma[1],
                PersonCount = splitDataByComma[2],
                CarCount = splitDataByComma[3],
                TruckCount = splitDataByComma[4],
                BikeCount = splitDataByComma[5],
                MiscCount = splitDataByComma[6],
            };

            zonesDataList.Add(zoneData);
        }

        return zonesDataList;
    }

    #endregion

    #region Camera

    public async Task<List<Camera>> GetCamerasAsync()
    {
        var result = await GetCachedCamerasAsync();

        return result is null ? new List<Camera>() : result;
    }

    public async Task<int> GetCamerasCountAsync()
    {
        var cameras = await GetCachedCamerasAsync();
        return cameras.Count();
    }

    public async ValueTask<Camera?> GetCameraByIdAsync(int id)
    {
        var cameras = await GetCachedCamerasAsync();
        var camera = cameras.FirstOrDefault(e => e.Id == id);

        return camera is null ? null : camera;
    }

    public async Task<List<Camera>> GetCamerasByDeviceIdAsync(int deviceId)
    {
        var result = await GetCamerasAsync();
        return result.FindAll(d => d.DeviceId == deviceId);
    }

    public async Task<int> AddCameraAsync(CameraVMCreateEdit vm)
    {
        var ctx = GetAppDbContext();

        var camera = new Camera
        {
            Name = vm.Name,
            Index = vm.Index,
            Resolution = vm.Resolution,
            DeviceId = vm.DeviceId,
            Zones = vm.Zones,
        };


        await ctx.Cameras.AddAsync(camera);

        var result = await ctx.SaveChangesAsync();
        if (result > 0)
        {

            ResetCachedCameras();
            return camera.Id;
        }

        return 0;
    }

    public async Task<bool> UpdateCameraAsync(CameraVMCreateEdit vm)
    {
        var ctx = GetAppDbContext();

        var camera = await ctx.Cameras
            .FirstOrDefaultAsync(n => n.Id == vm.Id);

        if (camera is null)
            return false;

        camera.Name = vm.Name;
        camera.Index = vm.Index;
        camera.Resolution = vm.Resolution;
        camera.DeviceId = vm.DeviceId;
        camera.Zones = vm.Zones;

        var result = await ctx.SaveChangesAsync();
        if (result > 0)
        {
            ResetCachedCameras();
            return true;
        }

        return false;
    }


    public async Task<bool> DeleteCameraFromDBAsync(int Id)
    {
        var ctx = GetAppDbContext();

        var camera = await ctx.Cameras
            .FirstOrDefaultAsync(n => n.Id == Id);

        if (camera is null)
            return false;

        ctx.Remove(camera);

        var result = await ctx.SaveChangesAsync();
        if (result > 0)
        {
            ResetCachedCameras();
        }

        return result > 0;
    }

    #endregion

    #region CameraLogs

    public async Task<int> GetCameraLogsCountAsync(
        int? deviceId = null,
        string? search = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int offset = 0)
    {
        var ctx = GetAppDbContext();

        IQueryable<CameraLog> cameraLogQuery = ctx.CameraLogs
            .AsNoTracking()
            .AsQueryable();

        if (deviceId != null && deviceId > 0)
        {
            cameraLogQuery = cameraLogQuery.Where(log => log.DeviceId == deviceId);
        }

        if (search != null)
        {
            //cameraLogQuery = cameraLogQuery.Where(n => n.Data["CameraIndex"] == int.Parse(search));
            var searchedCamDevice = ctx.Cameras.FirstOrDefault(c => c.Name == search);
            if(searchedCamDevice != null)
            {
                cameraLogQuery = cameraLogQuery.Where(n => n.CameraId == searchedCamDevice.Id);
            }
        }


        if (startDate != null )
        {
            cameraLogQuery = cameraLogQuery.Where(n => n.Timestamp >= startDate.Value.AddMinutes(offset));
        }

        if (endDate != null)
        {
            cameraLogQuery = cameraLogQuery.Where(n => n.Timestamp <= endDate.Value.AddMinutes(offset));
        }

        return await cameraLogQuery.CountAsync();
    }

    public async Task<List<CameraLogVM>> GetCameraLogsAsync(
        int? deviceId = null,
        string? search = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int pageNumber = 1,
        int pageSize = 50,
        int offset = 0)
    {
        var ctx = GetAppDbContext();

        var devicesList = ctx.Devices
            .Include(e => e.Cameras)
            .ToList();

        IQueryable<CameraLog> cameraLogQuery = ctx.CameraLogs
            .AsNoTracking()
            .AsQueryable();

        if (deviceId != null && deviceId > 0)
        {
            cameraLogQuery = cameraLogQuery.Where(log => log.DeviceId == deviceId);
        }

        if (search != null)
        {
            //cameraLogQuery = cameraLogQuery.Where(n => n.Data["CameraIndex"] == int.Parse(search));
            var searchedCamDevice = ctx.Cameras.FirstOrDefault(c => c.Name == search);
            if (searchedCamDevice != null)
            {
                cameraLogQuery = cameraLogQuery.Where(n => n.CameraId == searchedCamDevice.Id);
            }
        }

        if (startDate != null)
        {
            cameraLogQuery = cameraLogQuery.Where(n => n.Timestamp >= startDate.Value.AddMinutes(offset));
        }

        if (endDate != null)
        {
            cameraLogQuery = cameraLogQuery.Where(n => n.Timestamp <= endDate.Value.AddMinutes(offset));
        }


        var logIds = await cameraLogQuery
            .OrderByDescending(n => n.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var cameraLogs = logIds
            .Join(devicesList, camLog => camLog.DeviceId, dev => dev.Id, (camLog, dev) =>
            new CameraLogVM
            {
                Id = camLog.Id,
                DeviceName = dev.Name,
                CameraName = dev.Cameras.FirstOrDefault(c => c.Id == camLog.CameraId).Name,
                ImagePath = camLog.ImagePath,
                ZoneId = camLog.Data["ZoneId"],
                PersonCount = camLog.Data["PersonCount"],
                CarCount = camLog.Data["CarCount"],
                TruckCount = camLog.Data["TruckCount"],
                BikeCount = camLog.Data["BikeCount"],
                MiscCount = camLog.Data["MiscCount"],
                Timestamp = camLog.Timestamp,
            }).ToList();

        return cameraLogs is null ? Enumerable.Empty<CameraLogVM>().ToList() : cameraLogs;
    }

    public async Task<bool> AddCameraLogAsync(CameraLog log)
    {
        var ctx = GetAppDbContext();

        await ctx.CameraLogs.AddAsync(log);

        var result = await ctx.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> DeleteCameraLogAsync(int Id)
    {
        var ctx = GetAppDbContext();

        var cameraLog = await ctx.CameraLogs
            .FirstOrDefaultAsync(n => n.Id == Id);

        if (cameraLog is null)
            return false;


        var imagePath = cameraLog.ImagePath;
        if (!string.IsNullOrEmpty(imagePath))
        {
            var fullPath = Path.Combine(_hostingEnvironment.WebRootPath, imagePath.TrimStart('/'));
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

        ctx.Remove(cameraLog);

        var result = await ctx.SaveChangesAsync();
        return result > 0;
    }

    #endregion

    #region Logs

    public async Task<int> GetDeviceLogsCountAsync(
        int? deviceId = null,
        string? search = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int offset = 0)
    {
        var ctx = GetAppDbContext();

        IQueryable<Log> logQuery = ctx.Logs
            .AsNoTracking()
            .AsQueryable();

        if (deviceId != null && deviceId > 0)
        {
            logQuery = logQuery.Where(log => log.DeviceId == deviceId);
        }

        //if (search != null)
        //{
        //    logQuery = logQuery.Where(n => n.Parameters.ToUpper().Contains(search));
        //}

        if (search != null)
        {
            //cameraLogQuery = cameraLogQuery.Where(n => n.Data["CameraIndex"] == int.Parse(search));
            var searchedLogDevice = ctx.Cameras.FirstOrDefault(c => c.Name == search);
            if (searchedLogDevice != null)
            {
                logQuery = logQuery.Where(n => n.DeviceId == searchedLogDevice.DeviceId);
            }
        }

        if (startDate != null)
        {
            logQuery = logQuery.Where(n => n.Timestamp >= startDate.Value.AddMinutes(offset));
        }

        if (endDate != null)
        {
            logQuery = logQuery.Where(n => n.Timestamp <= endDate.Value.AddMinutes(offset));
        }

        return await logQuery.CountAsync();
    }

    public async Task<List<LogVM>> GetDeviceLogsAsync(
        int? deviceId = null,
        string? search = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int pageNumber = 1,
        int pageSize = 50,
        int offset = 0)
    {
        var ctx = GetAppDbContext();

        var devicesList = ctx.Devices
            .Include(e => e.Cameras)
            .ToList();

        IQueryable<Log> logQuery = ctx.Logs
            .AsNoTracking()
            .AsQueryable();

        if (deviceId != null && deviceId > 0)
        {
            logQuery = logQuery.Where(log => log.DeviceId == deviceId);
        }

        if (search != null)
        {
            //cameraLogQuery = cameraLogQuery.Where(n => n.Data["CameraIndex"] == int.Parse(search));
            var searchedLogDevice = ctx.Cameras.FirstOrDefault(c => c.Name == search);
            if (searchedLogDevice != null)
            {
                logQuery = logQuery.Where(n => n.DeviceId == searchedLogDevice.DeviceId);
            }
        }
        if (startDate != null)
        {
            logQuery = logQuery.Where(n => n.Timestamp >= startDate.Value.AddMinutes(offset));
        }

        if (endDate != null)
        {
            logQuery = logQuery.Where(n => n.Timestamp <= endDate.Value.AddMinutes(offset));
        }

        var logIds = await logQuery
            .OrderByDescending(n => n.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var logs = logIds
            .Join(devicesList, log => log.DeviceId, dev => dev.Id, (log, dev) =>
            new LogVM
            {
                Id = log.Id,
                DeviceName = dev.Name,
                Location = dev.Location,
                Camera = dev.Cameras.FirstOrDefault(idx =>
                    idx.Index == (log.Parameters.Substring(0, 1) != "c" ? int.Parse(log.Parameters.Substring(0, 1)) : 0))?.Name,
                Parameters = log.Parameters,
                Type = log.Type,
                Timestamp = log.Timestamp,
            }).ToList();

        return logs is null ? Enumerable.Empty<LogVM>().ToList() : logs;
    }



    public async Task<int> AddLogAsync(Log log)
    {
        var ctx = GetAppDbContext();

        await ctx.Logs.AddAsync(log);

        var result = await ctx.SaveChangesAsync();
        return result > 0 ? log.Id : 0;
    }

    public async Task<bool> DeleteLogAsync(int Id)
    {
        var ctx = GetAppDbContext();

        var log = await ctx.Logs
            .FirstOrDefaultAsync(n => n.Id == Id);

        if (log is null)
            return false;

        ctx.Remove(log);

        var result = await ctx.SaveChangesAsync();
        return result > 0;
    }

    #endregion

    #region Identity

    public async Task<List<AppRole>> GetAppRolesAsync()
    {
        var roles = await GetCachedAppRolesAsync();
        return roles;
    }

    public async Task<List<AppUser>> GetAppUsersAsync()
    {
        var users = await GetCachedAppUsersAsync();
        return users;
    }

    public async ValueTask<AppUser?> GetAppUserByUsernameAsync(string username)
    {
        var users = await GetCachedAppUsersAsync();
        return users.First(u => u.UserName == username);
    }

    public async Task<int> GetUsersCountAsync()
    {
        var users = await GetCachedAppUsersAsync();
        return users.Count();
    }

    #endregion

    #region Caching

    private async ValueTask<List<AppUser>> GetCachedAppUsersAsync()
    {
        if (_cache.TryGetValue(Constants.CACHE_USERS, out List<AppUser>? fromCache))
        {
            if (fromCache != null)
            {
                return fromCache;
            }
        }

        var ctx = GetAppDbContext();

        var toCache = await ctx.Users
            .AsNoTracking()
            .OrderBy(u => u.Id)
            .ToListAsync();

        _cache.Set(Constants.CACHE_USERS, toCache, TimeSpan.FromMinutes(60));

        return toCache;
    }

    public void ResetCachedAppUsers()
    {
        _cache.Remove(Constants.CACHE_USERS);
    }

    private async ValueTask<List<AppRole>> GetCachedAppRolesAsync()
    {
        if (_cache.TryGetValue(Constants.CACHE_ROLES, out List<AppRole>? rolesFromCache))
        {
            if (rolesFromCache != null)
            {
                return rolesFromCache;
            }
        }

        var ctx = GetAppDbContext();

        var roles = await ctx.Roles
            .AsNoTracking()
            .ToListAsync();

        _cache.Set(Constants.CACHE_ROLES, roles, TimeSpan.FromMinutes(60));

        return roles;
    }

    public void ResetCachedAppRoles()
    {
        _cache.Remove(Constants.CACHE_ROLES);
    }

    private async ValueTask<List<Device>> GetCachedDevicesAsync()
    {
        if (_cache.TryGetValue(Constants.CACHE_DEVICES, out List<Device>? devicesFromCache))
        {
            if (devicesFromCache != null)
            {
                return devicesFromCache;
            }
        }

        var ctx = GetAppDbContext();

        var devices = await ctx.Devices
            .AsNoTracking()
            .OrderBy(d => d.Network)
            .ThenBy(d => d.Node)
            .ToListAsync();

        _cache.Set(Constants.CACHE_DEVICES, devices, TimeSpan.FromMinutes(60));

        return devices;
    }

    private void ResetCachedDevices()
    {
        _cache.Remove(Constants.CACHE_DEVICES);
    }

    private async ValueTask<List<Camera>> GetCachedCamerasAsync()
    {
        if (_cache.TryGetValue(Constants.CACHE_CAMERAS, out List<Camera>? fromCache))
        {
            if (fromCache != null)
            {
                return fromCache;
            }
        }

        var ctx = GetAppDbContext();

        var cameras = await ctx.Cameras
            .AsNoTracking()
            .Include(d => d.Device)
            .OrderBy(d => d.DeviceId)
            .ToListAsync();

        _cache.Set(Constants.CACHE_CAMERAS, cameras, TimeSpan.FromMinutes(60));

        return cameras;
    }

    private void ResetCachedCameras()
    {
        _cache.Remove(Constants.CACHE_CAMERAS);
    }

    private async ValueTask<List<CameraLog>> GetCachedCameraLogsAsync()
    {
        if (_cache.TryGetValue(Constants.CACHE_CAMERA_LOGS, out List<CameraLog>? cameraLogsFromCache))
        {
            if (cameraLogsFromCache != null)
            {
                return cameraLogsFromCache;
            }
        }

        var ctx = GetAppDbContext();

        var devices = await ctx.CameraLogs
            .AsNoTracking()
            .OrderByDescending(d => d.Id)
            .ThenBy(d => d.DeviceId)
            .ThenBy(d => d.CameraId)
            .Take(100)
            .ToListAsync();

        _cache.Set(Constants.CACHE_CAMERA_LOGS, devices, TimeSpan.FromMinutes(60));

        return devices;
    }

    private void ResetCachedCachedLogs()
    {
        _cache.Remove(Constants.CACHE_CAMERA_LOGS);
    }

    private async ValueTask<DashboardDTO> GetCachedDashboardAsync()
    {
        if (_cache.TryGetValue(Constants.CACHE_DASHBOARD, out DashboardDTO? fromCache))
        {
            if (fromCache != null)
            {
                return fromCache;
            }
        }

        var devices = await GetCachedDevicesAsync();
        var camLogsInDb = await GetCachedCameraLogsAsync();

        List<CameraLog> camLogs = new();
        foreach (var device in devices)
        {
            if (device.Cameras != null)
            {
                foreach (var cam in device.Cameras)
                {
                    var camLog = camLogsInDb.FirstOrDefault(c => c.DeviceId == device.Id && c.CameraId == cam.Id);
                    if (camLog != null)
                    {
                        camLogs.Add(camLog);
                    }
                }
            }
        }

        var dashboardData = new DashboardDTO { Devices = devices, CameraLogs = camLogs };

        _cache.Set(Constants.CACHE_DASHBOARD, dashboardData, TimeSpan.FromMinutes(60));

        return dashboardData;
    }

    private void ResetCachedDashboard()
    {
        _cache.Remove(Constants.CACHE_DASHBOARD);
    }

    #endregion
}
