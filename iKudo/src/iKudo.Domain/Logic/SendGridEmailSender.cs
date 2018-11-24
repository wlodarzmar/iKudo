using iKudo.Domain.Configuration;
using iKudo.Domain.Interfaces;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace iKudo.Domain.Logic
{
    public class SendGridEmailSender : ISendEmails
    {
        private readonly SendGridClient client;

        public SendGridEmailSender(IOptions<SendGridConfig> options)
        {
            client = new SendGridClient(options.Value.ApiKey);
        }

        public HttpStatusCode Send(string subject, string content, string fromEmail, string[] toEmails)
        {
            return SendAsync(subject, content, fromEmail, toEmails).Result;
        }

        public List<KeyValuePair<string, HttpStatusCode>> Send(IEnumerable<MailMessage> mails)
        {
            return SendAsync(mails).Result;
        }

        public async Task<HttpStatusCode> SendAsync(string subject, string content, string fromEmail, string[] toEmails)
        {
            var fromAddress = new EmailAddress(fromEmail);
            var emailsToSend = toEmails.Select(x => new EmailAddress(x)).ToList();
            SendGridMessage message = MailHelper.CreateSingleEmailToMultipleRecipients(fromAddress, emailsToSend, subject, string.Empty, content);
            Response task = await client.SendEmailAsync(message);

            return task.StatusCode;
        }

        public async Task<List<KeyValuePair<string, HttpStatusCode>>> SendAsync(IEnumerable<MailMessage> mails)
        {
            if (!mails.Any())
            {
                return new List<KeyValuePair<string, HttpStatusCode>>();
            }

            var results = new List<KeyValuePair<string, HttpStatusCode>>();
            foreach (var mail in mails)
            {
                var message = MailHelper.CreateSingleEmail(new EmailAddress(mail.From.Address),
                                                           new EmailAddress(mail.To.First().Address),
                                                           mail.Subject, string.Empty, mail.Body);
                var task = await client.SendEmailAsync(message);
                results.Add(new KeyValuePair<string, HttpStatusCode>(mail.To.First().Address, task.StatusCode));
            }

            return results;
        }
    }
}