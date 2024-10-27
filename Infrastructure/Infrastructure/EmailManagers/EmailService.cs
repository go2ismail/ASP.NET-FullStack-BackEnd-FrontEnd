// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Services.CQS.Queries;
using Application.Services.Externals;

using MailKit.Net.Smtp;

using Microsoft.EntityFrameworkCore;

using MimeKit;

namespace Infrastructure.EmailManagers;

public class EmailService(IQueryContext context, IEncryptionService encryptionService) : IEmailService
{
    private readonly IQueryContext _context = context;
    private readonly IEncryptionService _encryptionService = encryptionService;

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var defaultConfig = await _context.Config
            .ApplyIsDeletedFilter()
            .Where(x => x.Active == true)
            .SingleOrDefaultAsync();

        if (defaultConfig != null)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("noreply", ""));
            message.To.Add(new MailboxAddress(email, email));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = htmlMessage
            };

            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(defaultConfig.SmtpHost, defaultConfig.SmtpPort, true);
                await client.AuthenticateAsync(defaultConfig.SmtpUserName, _encryptionService.Decrypt(defaultConfig.SmtpPassword));
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
