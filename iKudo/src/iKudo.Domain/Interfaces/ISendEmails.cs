using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace iKudo.Domain.Interfaces
{
    public interface ISendEmails
    {
        HttpStatusCode Send(string subject, string content, string fromEmail, string[] toEmails);
        Task<HttpStatusCode> SendAsync(string subject, string content, string fromEmail, string[] toEmails);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mails"></param>
        /// <returns>List of key value pairs - key: email: value: status code</returns>
        List<KeyValuePair<string, HttpStatusCode>> Send(IEnumerable<MailMessage> mails);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mails"></param>
        /// <returns>List of key value pairs - key: email: value: status code</returns>
        Task<List<KeyValuePair<string, HttpStatusCode>>> SendAsync(IEnumerable<MailMessage> mails);
    }
}
