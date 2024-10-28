// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Services.Externals;

using MediatR;

using System.Text.Encodings.Web;

namespace Application.Features.Accounts.Events;

public class RegisterUserEventHandler(IEmailService emailService) : INotificationHandler<RegisterUserEvent>
{
    private readonly IEmailService _emailService = emailService;

    public async Task Handle(RegisterUserEvent notification, CancellationToken cancellationToken)
    {
        if (!notification.SendEmailConfirmation)
        {
            Console.WriteLine($"Handling event for: {notification.Email}. Not sending email confirmation");
        }
        else
        {

            Console.WriteLine($"Handling event for: {notification.Email}. Sending email confirmation");

            var callbackUrl = $"{notification.Host}/Accounts/ConfirmEmail?email={notification.Email}&code={notification.EmailConfirmationToken}";
            var encodeCallbackUrl = $"{HtmlEncoder.Default.Encode(callbackUrl)}";

            var emailSubject = $"Confirm your email";
            var emailMessage = $"Please confirm your account by <a href='{encodeCallbackUrl}'>clicking here</a>.";

            await _emailService.SendEmailAsync(notification.Email, emailSubject, emailMessage);
        }
    }
}
