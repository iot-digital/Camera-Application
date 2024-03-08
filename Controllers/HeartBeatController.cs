using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ParkingDemo.Data;
using ParkingDemo.DTOs;
using ParkingDemo.Hubs;
using ParkingDemo.Models;
using ParkingDemo.Services;

namespace ParkingDemo.Controllers
{
    [Route("/Dashboard")]
    [ApiController]
    public class HeartBeatController : ControllerBase
    {
        private readonly MessageQueueService _queue;
        private readonly AppRepository _repository;
        private readonly IHubContext<StatusHub> _dashboardHub;

        public HeartBeatController(MessageQueueService queue, AppRepository repo, IHubContext<StatusHub> dashboardHub)
        {
            _queue = queue;
            _repository = repo;
            _dashboardHub = dashboardHub;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Message message)
        {
            await ProcessMessageAndBroadcast(message);

            return Ok();
        }

        private async Task ProcessMessageAndBroadcast(Message message)
        {
            if (message == null)
                return;

            switch (message.ContentType)
            {
                case "h":
                    var pingResult = await _repository
                        .UpdateDeviceAlivePingAsync(message.Network, message.Node, message.ContentValue);

                    if (pingResult)
                    {
                        await SendDataToHub($"{message.Network}.{message.Node}", "ONLINE");
                        await SendDataToApplication(message, new List<Dictionary<int, string>>());
                    }

                    break;
                default:

                    break;
            }
        }

        private async Task SendDataToApplication(Message message, List<Dictionary<int, string>> imgPaths)
        {
            await _dashboardHub.Clients.All.SendAsync("ReceiveMessage", message, imgPaths);
        }

        private async Task SendDataToHub(string deviceAddress, string status)
        {
            await _dashboardHub.Clients.All.SendAsync("ReceiveMessage", deviceAddress, status);
        }
    }
}
