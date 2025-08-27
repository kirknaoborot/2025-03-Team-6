using Infrastructure.Shared.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;


namespace Auth.Application.Consumer
{
    public class UserLoggedInEventHandler : IConsumer<UserLoggedInEvent>
    {
        private readonly ILogger<UserLoggedInEventHandler> _logger;

        public UserLoggedInEventHandler(ILogger<UserLoggedInEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<UserLoggedInEvent> context)
        {
            var @event = context.Message;
            _logger.LogInformation("Пользователь вошёл: {Login}, {FullName}, Время: {LoginTime}",
                @event.Login, @event.FullName, @event.LoginTime);
            
            await Task.Delay(60_000);

            //return Task.CompletedTask;
        }
    }
}
