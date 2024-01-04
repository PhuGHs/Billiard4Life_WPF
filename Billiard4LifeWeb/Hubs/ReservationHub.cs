using Microsoft.AspNetCore.SignalR;

namespace Billiard4LifeWeb.Hubs
{
    public class ReservationHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            Console.WriteLine("Hub connected!");

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine("Hub disconnected!");

            return base.OnDisconnectedAsync(exception);
        }
    }
}
