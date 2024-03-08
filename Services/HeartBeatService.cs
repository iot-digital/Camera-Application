using Microsoft.AspNetCore.SignalR;
using ParkingDemo.Data;
using ParkingDemo.Hubs;
using ParkingDemo.Utilities;

namespace ParkingDemo.Services
{
    public class HeartbeatService : BackgroundService, IDisposable
    {
        private readonly PeriodicTimer _timer;
        private readonly IHubContext<StatusHub> _hubContext;
        private readonly AppRepository _repository;

        public HeartbeatService(IHubContext<StatusHub> hubContext, AppRepository repository)
        {
            _hubContext = hubContext;
            _repository = repository;
            _timer = new PeriodicTimer(TimeSpan.FromSeconds(Constants.ALIVE_INTERVAL));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (await _timer.WaitForNextTickAsync(stoppingToken)
                && !stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine("60 seconds timer reached");

                var devices = await _repository.GetDevicesAsync();

                foreach (var device in devices)
                {
                    if(device.LastAlivePing == null)
                    {
                        continue;
                    }

                    DateTime currentTime = DateTime.UtcNow;
                    TimeSpan timeDiff = (TimeSpan)(currentTime - device.LastAlivePing);

                    if (timeDiff.TotalSeconds > 60)
                    {
                        await SendDataToHub($"{device.Address}", "OFFLINE");
                    }
                }
            }
        }

        public override void Dispose()
        {
            _timer.Dispose();
            base.Dispose();
        }

        private async Task SendDataToHub(string deviceAddress, string status)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", deviceAddress, status);
        }
    }
}