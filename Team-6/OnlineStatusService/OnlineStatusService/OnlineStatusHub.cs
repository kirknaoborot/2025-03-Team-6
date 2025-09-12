using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStatusService
{
    public class OnlineStatusHub : Hub
    {
        public async Task UserOnline(string userId)
        {
            Console.WriteLine($"Пользователь {userId} онлайн");

            // Здесь можно добавить код для RabbitMQ, например:
            // await _rabbitService.EnqueueUserOnline(userId);

            await Clients.Others.SendAsync("UserCameOnline", userId);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"Пользователь отключился: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
