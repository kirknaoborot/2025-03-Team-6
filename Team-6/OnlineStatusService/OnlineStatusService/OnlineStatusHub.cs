using Infrastructure.Shared.Contracts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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


        public async Task UserOnline(string userId)
        {
            Console.WriteLine($"Пользователь {userId} онлайн");

            var obj = JObject.Parse(userId);
            var id = obj["id"]?.Value<string>();

            var agentStatusEvent = new AgentStatusEvent
            {
                AgentId = Guid.TryParse(id, out var result) ? result : Guid.Empty,
                Date = DateTime.UtcNow,
                Status = Infrastructure.Shared.Enums.AgentStatusType.Connect
            };

            await _publishEndpoint.Publish(agentStatusEvent);

            await Clients.Others.SendAsync("UserCameOnline", id);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"Пользователь отключился: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
