using MailKit.Net.Smtp;
using MimeKit;
using UNP.Data;

namespace UNP.Services
{
    public interface IEmailService
    {
        Task SendEmails();
    }
    public class EmailService : IEmailService
    {
        private readonly AppDbContext _context;

        public EmailService(AppDbContext context)
        {
            _context = context;
        }

        public async Task SendEmails()
        {
            var changes = _context.UnpHistoryChanges
                .GroupBy(c => c.Email)
                .ToList();

            int emailCount = 0;
            const int batchSize = 100;

            foreach (var changeGroup in changes)
            {
                var email = changeGroup.Key;
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("UNP", "yourapp@example.com"));
                message.To.Add(new MailboxAddress("", email));
                message.Subject = "UNP Status Changes";

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = "<h1>UNP - Изменения статусов</h1><ul>"
                };

                foreach (var change in changeGroup)
                {
                    bodyBuilder.HtmlBody += $"<li>{change.Unp}: {change.ChangeType}</li>";
                }

                bodyBuilder.HtmlBody += "</ul>";

                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync("smtp.example.com", 587, false);
                await client.AuthenticateAsync("yourapp@example.com", "password");
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                // Удаление изменений после отправки email
                _context.UnpHistoryChanges.RemoveRange(changeGroup);

                emailCount++;

                // Если отправлено 100 писем, делаем перерыв на 5 минут
                if (emailCount % batchSize == 0)
                {
                    await Task.Delay(5 * 60 * 1000);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
