using AutocenterAPI.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;

namespace AutocenterAPI.Services {
    public class SmtpEmailService : IEmailService {
        private readonly IConfiguration _config;
        public SmtpEmailService(IConfiguration config) => _config = config;

        public async Task SendAsync(string toEmail, string subject, string htmlBody) {
            var fromName = _config["Smtp:FromName"] ?? "Library";
            var fromEmail = _config["Smtp:FromEmail"];

            if (string.IsNullOrWhiteSpace(fromEmail))
                throw new InvalidOperationException("SMTP FromEmail not configured.");

            var msg = new MimeMessage();
            msg.From.Add(new MailboxAddress(fromName, fromEmail));
            msg.To.Add(MailboxAddress.Parse(toEmail));
            msg.Subject = subject;

            var body = new BodyBuilder { HtmlBody = htmlBody };
            msg.Body = body.ToMessageBody();

            using var client = new SmtpClient();

            await client.ConnectAsync(
                _config["Smtp:Host"],
                int.Parse(_config["Smtp:Port"] ?? "587"),
                MailKit.Security.SecureSocketOptions.StartTls);

            await client.AuthenticateAsync(
                _config["Smtp:User"],
                _config["Smtp:Pass"]);

            await client.SendAsync(msg);
            await client.DisconnectAsync(true);
        }
    }
}