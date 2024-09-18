using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using api.Helpers;
using api.Interfaces;

namespace api.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendConfirmationEmailAsync(string email, string confirmationLink)
        {
            var smtpServer = _configuration["SmtpSettings:Server"];
            if (string.IsNullOrWhiteSpace(smtpServer))
            {
                throw new ArgumentException("SMTP Server is not configured.");
            }

            var smtpPortValue = _configuration["SmtpSettings:Port"];
            if (!int.TryParse(smtpPortValue, out var smtpPort) || smtpPort <= 0)
            {
                throw new ArgumentException("Invalid SMTP Port.");
            }

            var smtpUsername = _configuration["SmtpSettings:Username"];
            if (string.IsNullOrWhiteSpace(smtpUsername))
            {
                throw new ArgumentException("SMTP Username is not configured.");
            }

            var smtpPassword = _configuration["SmtpSettings:Password"];
            if (string.IsNullOrWhiteSpace(smtpPassword))
            {
                throw new ArgumentException("SMTP Password is not configured.");
            }


            using var client = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = new System.Net.NetworkCredential(smtpUsername, smtpPassword),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpUsername),
                Subject = "Confirm your email",
                Body = $"Please confirm your email by clicking this link: {confirmationLink}",
                IsBodyHtml = true,
            };
            mailMessage.To.Add(email);

            await client.SendMailAsync(mailMessage);
        }
    }
}