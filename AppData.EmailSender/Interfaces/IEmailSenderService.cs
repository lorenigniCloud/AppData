using Microsoft.Extensions.Logging;

namespace AppData.EmailSender.Interfaces
{
    public interface IEmailSenderService
    {
        Task SendMailAsync(string body, string subject, string userMail);
    }
}
