using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentPortal.Services.Email
{
    public class EmailService : IEmail
    {
        private readonly SmtpClient _smtpClient;
        private readonly IConfiguration _configuration;
     
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;

            // Initialize SmtpClient with configuration values
            string smtpServer = _configuration["SmtpSettings:Server"];
            int smtpPort = _configuration.GetValue<int>("SmtpSettings:Port", 587); // Default port 587
            string smtpUsername = _configuration["SmtpSettings:Username"];
            string smtpPassword = _configuration["SmtpSettings:Password"];

            if (string.IsNullOrEmpty(smtpServer) || string.IsNullOrEmpty(smtpUsername) || string.IsNullOrEmpty(smtpPassword))
            {
                throw new ApplicationException("SMTP configuration is incomplete. Ensure SmtpSettings section in appsettings.json is properly configured.");
            }

            _smtpClient = new SmtpClient
            {
                Host = smtpServer,
                Port = smtpPort,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(smtpUsername, smtpPassword)
            };
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                var mail = new MailMessage
                {
                    IsBodyHtml = true,
                    Body = body,
                    Subject = subject,
                    Priority = MailPriority.High,
                    From = new MailAddress(_configuration["SmtpSettings:SenderEmail"])
                };
                mail.To.Add(new MailAddress(to));

                await _smtpClient.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                throw new ApplicationException("Failed to send email", ex);
            }
        }

        public void Dispose()
        {
            _smtpClient.Dispose();
        }
    }
}
