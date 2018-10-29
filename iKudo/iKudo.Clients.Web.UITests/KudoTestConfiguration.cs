using Microsoft.Extensions.Configuration;

namespace iKudo.Clients.Web.UITests
{
    public class KudoTestConfiguration
    {
        public KudoTestConfiguration(IConfiguration configuration)
        {
            KudoPageUrl = configuration["AppSettings:KudoWebUrl"];
            User1Email = configuration["AppSettings:User1Email"];
            User1Password = configuration["AppSettings:User1Password"];
            User2Email = configuration["AppSettings:User2Email"];
            User2Password = configuration["AppSettings:User2Password"];
            TempMailUrl = configuration["AppSettings:TempMailUrl"];
            WebDriverArguments = configuration["AppSettings:WebDriverArguments"];
        }

        public string KudoPageUrl { get; }
        public string User1Email { get; }
        public string User1Password { get; }
        public string User2Email { get; }
        public string User2Password { get; }
        public string TempMailUrl { get; }
        public string WebDriverArguments { get; }
    }
}
