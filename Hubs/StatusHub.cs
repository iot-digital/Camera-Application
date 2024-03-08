using Microsoft.AspNetCore.SignalR;

namespace ParkingDemo.Hubs;

public class StatusHub : Hub
{
    public async Task SendMessage(string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", message);
    }
}

public class StateHub : Hub
{
    public async Task SendMessage(string message)
    {
        await Clients.All.SendAsync("ReceiveData", message);
    }
}