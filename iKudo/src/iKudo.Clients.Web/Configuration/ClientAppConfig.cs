namespace iKudo.Clients.Web.Configuration
{
    public class ClientAppConfig
    {
        public ClientAppConfig()
        {
            Auth0Config = new Auth0ClientConfig();
        }

        public string InvitationAcceptUrlFormat { get; set; }

        public Auth0ClientConfig Auth0Config { get; set; }

        public string IkudoPageUrl { get; set; }
    }

    public class Auth0ClientConfig
    {
        public string ReturnUrl { get; set; }

        public string ClientId { get; set; }

        public string Domain { get; set; }

        public string Audience { get; set; }

    }
}
