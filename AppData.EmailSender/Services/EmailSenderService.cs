using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AppData.EmailSender.Interfaces;
using AppData.EmailSender.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AppData.EmailSender.Services
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly SmtpConfiguration _smtpConfig;

        public EmailSenderService(IOptions<SmtpConfiguration> smtpConfig, ILogger<EmailSenderService> logger)
        {
            _smtpConfig = smtpConfig?.Value ??
                throw new ArgumentNullException(nameof(smtpConfig));

            logger.LogInformation($"SMTP Configuration - Sender: {_smtpConfig.Sender}");
            logger.LogInformation($"SMTP Configuration - Host: {_smtpConfig.Host}");

            if (string.IsNullOrEmpty(_smtpConfig.Sender))
                throw new InvalidOperationException("SMTP Sender configuration is missing");
        }

        public async Task SendMailAsync(string body, string subject, string userMail)
        {
            if (string.IsNullOrEmpty(_smtpConfig.Sender))
                throw new InvalidOperationException("SMTP Sender configuration is missing");

            try
            {
                using var client = new SmtpClient
                {
                    Host = _smtpConfig.Host,
                    Port = Convert.ToInt32(_smtpConfig.Port),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_smtpConfig.User, _smtpConfig.Password)
                };

                var fromAddress = new MailAddress(_smtpConfig.Sender, _smtpConfig.SenderName);
                var toAddress = new MailAddress(userMail);

                using var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                };

                await client.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to send email: {ex.Message}", ex);
            }
        }
    }
}
