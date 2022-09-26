using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Nexus.OAuth.Domain.Messages
{
    public class EmailMessageSender
    {
        private readonly string apiKey;
        private readonly string senderEmail;
        private readonly string senderName;
        public EmailMessageSender(IConfiguration configuration)
        {
            var section = configuration
                  .GetSection("Sendgrid");

            senderEmail = section
                           .GetSection("SenderEmail")
                           .Value;

            senderName = section
                         .GetSection("SenderName")
                         .Value;

            apiKey = section
                .GetSection("ApiKey")
                .Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="htmlContent"></param>
        /// <param name="subject"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public async Task SendEmailAsync(string htmlContent, string subject, string to)
        {
            var client = new SendGridClient(apiKey);
            var from_email = new EmailAddress(senderEmail, senderName);
            var to_email = new EmailAddress(to);
            var msg = MailHelper.CreateSingleEmail(from_email, to_email, subject, string.Empty, htmlContent);

            await client.SendEmailAsync(msg);
        }
    }
}
