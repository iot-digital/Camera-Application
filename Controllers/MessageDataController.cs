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
    [Route("/CameraDashboard")]
    [ApiController]
    public class MessageDataController : ControllerBase
    {
        private readonly MessageQueueService _queue;
        private readonly AppRepository _repository;  
        private readonly IHubContext<StateHub> _dashboardHub;

        public MessageDataController(MessageQueueService queue, AppRepository repo,IHubContext<StateHub> dashboardHub)
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
                case "m":
                    bool msgResult;
                    List<Dictionary<int, string>> imgPaths = new List<Dictionary<int, string>>();

                    if (message.Images != null && message.Images.Count < 1)
                    {
                        // LoRa MODE
                        (msgResult, _) = await _repository
                            .UpdateDeviceValueAndAddLogAsync(message.Network, message.Node, message.ContentValue, LogType.Data_LoRa);
                    }
                    else
                    {
                        // Wi-Fi MODE
                        (msgResult, _, imgPaths) = await _repository
                        .UpdateDeviceValueAndAddCamLogAsync(message.Network, message.Node, message.ContentValue, message.Images, LogType.Data_WiFi);
                    }

                    if (msgResult)
                    {
                        await SendDataToApplication(message, imgPaths);
                    }

                    break;              

                case "e":
                    var device = await _repository.GetDeviceByAddressAsync(message.Network, message.Node);
                    if (device is null)
                        return;

                    Log log = new()
                    {
                        DeviceId = device.Id,
                        Type = LogType.Error,
                        Parameters = message.ContentValue ?? "N/A",
                        Timestamp = DateTime.UtcNow,
                        IsAcknowledged = true
                    };

                    await _repository.AddLogAsync(log);

                    break;

                case "c":
                    var configMsgTrackItem = _queue.GetMessageById(message.Id);
                    if (configMsgTrackItem is null)
                        return;

                    (var cAckResult, var cDeviceId) = await _repository
                        .UpdateAckStatusAsync(configMsgTrackItem);

                    if (cAckResult)
                    {
                        _queue.RemoveMessage(message.Id);
                        await SendDataToConfigHub(cDeviceId, isSuccess: true);
                    }
                    else
                    {
                        await SendDataToConfigHub(cDeviceId, isSuccess: false);
                    }

                    break;                

                default:
                    break;
            }
        }

        private async Task SendDataToConfigHub(int deviceId, bool isSuccess)
        {
            var result = new string[] { deviceId.ToString(), isSuccess.ToString() };
            await _dashboardHub.Clients.All.SendAsync("ReceiveConfigMessage", result);
        }

        private async Task SendDataToApplication(Message message, List<Dictionary<int, string>> imgPaths)
        {
            await _dashboardHub.Clients.All.SendAsync("ReceiveMessage", message, imgPaths);
        }
    }
}
