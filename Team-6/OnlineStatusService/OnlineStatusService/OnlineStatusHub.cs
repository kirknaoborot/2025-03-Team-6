using Infrastructure.Shared.Contracts;
using MassTransit;
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
        private readonly IPublishEndpoint _publishEndpoint;

        public OnlineStatusHub(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }


        public async Task UserOnline(Guid userId)
        {
            Console.WriteLine($"Пользователь {userId} онлайн");

            await _publishEndpoint.Publish(new AgentStatusEvent
            {
                AgentId = userId,
                Date = DateTime.UtcNow,
                Status = Infrastructure.Shared.Enums.AgentStatusType.Connect
            });

            await Clients.Others.SendAsync("UserCameOnline", userId);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"Пользователь отключился: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
