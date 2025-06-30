using MessageHubService.Domain.Entities;

namespace MessageHubService.Application.Interfaces
{
    public interface IMessageEvent
    {
        event EventHandler<MessageEventArgs> OnIncomingMessage;

        /// <summary>
        /// пока не реализовано оправление сообщение с стороны оператора, поэтому пока прокидываем MessageEventArgs
        /// </summary>
        event EventHandler<MessageEventArgs> OnOutgoingMessage;
    }
}
