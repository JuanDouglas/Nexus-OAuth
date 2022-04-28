using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Nexus.OAuth.Domain.Messages
{
    public class EmailMessageSender
    {
        private readonly string apiKey;
        private readonly string sender;
        public EmailMessageSender(IConfiguration configuration)
        {
            var section = configuration
                  .GetSection("Sendgrid");

            sender = section
                           .GetSection("Sender")
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
            var from_email = new EmailAddress(sender);
            var to_email = new EmailAddress(to);
            var msg = MailHelper.CreateSingleEmail(from_email, to_email, subject, string.Empty, htmlContent);

            var response = await client.SendEmailAsync(msg)
                .ConfigureAwait(false);
        }
    }
}
