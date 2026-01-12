using Application.Interface;
using SWD392.MessageBroker;

namespace Infrastructure.Persistence
{
    public class EmailRepository : IEmailRepository
    {
        public void AddEmail(EmailMessageDTO emailMessageDTO)
        {
            EmailDBMemo.EmailMessages.Add(emailMessageDTO);
        }

        public List<EmailMessageDTO> GetAllEmails()
        {
            return EmailDBMemo.EmailMessages;
        }
    }
}
