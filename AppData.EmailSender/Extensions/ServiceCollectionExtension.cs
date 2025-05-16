using AppData.EmailSender.Interfaces;
using AppData.EmailSender.Models;
using AppData.EmailSender.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AppData.EmailSender.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddEmailSenderService(this IServiceCollection services)
        {
            services.AddSingleton<IEmailSenderService, EmailSenderService>();
            services.AddOptions<SmtpConfiguration>().BindConfiguration("Smtp");
            return services;
        }
    }
}
