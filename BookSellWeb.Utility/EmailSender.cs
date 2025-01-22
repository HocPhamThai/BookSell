using Microsoft.AspNetCore.Identity.UI.Services;

namespace BookEcomWeb.Utility
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Email sender logic here
            return Task.CompletedTask;
        }
    }
}
