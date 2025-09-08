using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using TscLoanManagement.TSCDB.Application.Interfaces;
using TscLoanManagement.TSCDB.Core.Domain.Authentication;

namespace TscLoanManagement.TSCDB.Application.Features
{
    public class EmailService : IEmailService
    {
        private readonly MailSettings _mailSettings;
        private readonly IWebHostEnvironment _env;

        public EmailService(IOptions<MailSettings> mailSettings, IWebHostEnvironment env)
        {
            _mailSettings = mailSettings.Value;
            _env = env;
        }

        public async Task SendWelcomeEmailAsync(string toEmail, string username, string password)
        {
            string templatePath = Path.Combine(_env.WebRootPath, "EmailTemplates", "WelcomeEmail.html");
            string body = await File.ReadAllTextAsync(templatePath);

            // Replace placeholders
            body = body.Replace("{{Username}}", username)
                       .Replace("{{Password}}", password)
                       .Replace("{{Email}}", toEmail);

            using (var client = new SmtpClient(_mailSettings.Host, _mailSettings.Port))
            {
                client.Credentials = new NetworkCredential(_mailSettings.Username, _mailSettings.Password);
                client.EnableSsl = _mailSettings.EnableSSL;

                var message = new MailMessage
                {
                    From = new MailAddress(_mailSettings.SenderEmail, "TSC Portal"),
                    Subject = "Welcome to TSC Portal",
                    Body = body,
                    IsBodyHtml = true
                };

                message.To.Add(toEmail);

                await client.SendMailAsync(message);
            }
        }

        public async Task SendOtpEmailAsync(string toEmail, string otp)
        {
            string body = $@"
        <h2>Login OTP</h2>
        <p>Your OTP for login is: <strong>{otp}</strong></p>
        <p>This OTP will expire in 5 minutes.</p>";

            using (var client = new SmtpClient(_mailSettings.Host, _mailSettings.Port))
            {
                client.Credentials = new NetworkCredential(_mailSettings.Username, _mailSettings.Password);
                client.EnableSsl = _mailSettings.EnableSSL;

                var message = new MailMessage
                {
                    From = new MailAddress(_mailSettings.SenderEmail, "TSC Portal"),
                    Subject = "Your Login OTP",
                    Body = body,
                    IsBodyHtml = true
                };

                message.To.Add(toEmail);

                await client.SendMailAsync(message);
            }
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            using (var client = new SmtpClient(_mailSettings.Host, _mailSettings.Port))
            {
                client.Credentials = new NetworkCredential(_mailSettings.Username, _mailSettings.Password);
                client.EnableSsl = _mailSettings.EnableSSL;

                var message = new MailMessage
                {
                    From = new MailAddress(_mailSettings.SenderEmail, "TSC Portal"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                message.To.Add(toEmail);

                await client.SendMailAsync(message);
            }
        }


    }
}
