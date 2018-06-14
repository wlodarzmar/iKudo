using System.Net;
using System.Threading.Tasks;

namespace iKudo.Domain.Interfaces
{
    public interface ISendEmails
    {
        HttpStatusCode Send(string subject, string content, string fromEmail, string[] toEmails);
        Task<HttpStatusCode> SendAsync(string subject, string content, string fromEmail, string[] toEmails); //TODO: single email
        //TODO:  method with multiple email subjects, contents and emails
    }
}
