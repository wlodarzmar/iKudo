using iKudo.Domain.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace iKudo.Domain.Logic
{
    public class SendGridEmailSender : ISendEmails
    {
        private readonly SendGridClient client;

        public SendGridEmailSender(string apiKey)
        {
            client = new SendGridClient(apiKey);
        }

        public HttpStatusCode Send(string subject, string content, string fromEmail, string[] toEmails)
        {
            return SendAsync(subject, content, fromEmail, toEmails).Result;
        }

        public async Task<HttpStatusCode> SendAsync(string subject, string content, string fromEmail, string[] toEmails)
        {
            var fromAddress = new EmailAddress(fromEmail);
            var emailsToSend = toEmails.Select(x => new EmailAddress(x)).ToList();
            SendGridMessage message = MailHelper.CreateSingleEmailToMultipleRecipients(fromAddress, emailsToSend, subject, string.Empty, content);
            Response task = await client.SendEmailAsync(message);

            return task.StatusCode;
        }
    }
}