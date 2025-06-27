using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Auth.Application.HubSignalR
{
    [AllowAnonymous]
    public class MessageHub: Hub
    {
        public async Task SendMessage(string message)
        {
            var messageObject = new
            {
                id = Guid.NewGuid().ToString(),
                text = message
            };
            await Clients.All.SendAsync("ReceiveMessage", messageObject);
        }

        public async Task SendMultipleMessages(string[] messages)
        {
            var messageObjects = messages.Select(m => new
            {
                id = Guid.NewGuid().ToString(),
                text = m
            }).ToArray();
            await Clients.All.SendAsync("ReceiveMultipleMessages", messageObjects);
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("ReceiveMessage", new
            {
                id = Guid.NewGuid().ToString(),
                text = "Welcome to the chat!"
            });
            await base.OnConnectedAsync();
        }
    }
}
