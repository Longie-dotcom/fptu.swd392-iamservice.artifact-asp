using Application.Helper;
using Application.Interface.IPublisher;
using MassTransit;
using SWD392.MessageBroker;

namespace Infrastructure.MessageBroker.Publisher
{
    public class UserDeletePublisher : IUserDeletePublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public UserDeletePublisher(
            IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task PublishAsync(UserDeleteDTO dto)
        {
            ServiceLogger.Logging(
                Level.Infrastructure, $"Publishing user delete for user ID: {dto.UserID}");
            await _publishEndpoint.Publish(dto);
        }
    }
}
