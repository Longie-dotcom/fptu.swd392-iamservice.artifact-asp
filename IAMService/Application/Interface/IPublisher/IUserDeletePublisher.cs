using SWD392.MessageBroker;

namespace Application.Interface.IPublisher
{
    public interface IUserDeletePublisher
    {
        Task PublishAsync(UserDeleteDTO dto);
    }
}