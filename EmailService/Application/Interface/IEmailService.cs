using SWD392.MessageBroker;

namespace Application.Interface
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailMessageDTO email);
    }
}
