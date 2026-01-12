using SWD392.MessageBroker;

namespace Application.Interface
{
    public interface IEmailRepository
    {
        List<EmailMessageDTO> GetAllEmails();
        void AddEmail(EmailMessageDTO emailMessageDTO);
    }
}
