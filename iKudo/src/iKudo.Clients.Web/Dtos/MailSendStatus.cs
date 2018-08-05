using System.Net;

namespace iKudo.Clients.Web.Dtos
{
    public class MailSendStatus
    {
        public string Mail { get; set; }
        public HttpStatusCode Status { get; set; }
    }
}
