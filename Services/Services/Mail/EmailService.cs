using Core.Utils;
using IServices.Interfaces.Mail;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using ModelViews.Requests.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.Mail
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings emailSettings;

        public EmailService(IOptions<EmailSettings> options)
        {
            emailSettings = options.Value;
        }

        public async Task SendEmailAsync(MailRequestDTO mailrequest)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(emailSettings.Email);
            email.Subject = mailrequest.Subject;
            email.To.Add(MailboxAddress.Parse(mailrequest.ToEmail));

            var builder = new BodyBuilder
            {
                HtmlBody = mailrequest.Body
            };
            email.Body = builder.ToMessageBody();

            using var smtp = new MailKit.Net.Smtp.SmtpClient(); // ✅ Use MailKit SmtpClient
            await smtp.ConnectAsync(emailSettings.Host, emailSettings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(emailSettings.Email, emailSettings.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}