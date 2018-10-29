using Microsoft.Extensions.Configuration;

namespace iKudo.Clients.Web.UITests
{
    public class BaseTest
    {
        public BaseTest()
        {
            Configuration = InitConfiguration();
            KudoConfiguration = new KudoTestConfiguration(Configuration);
        }

        private static IConfiguration Configuration { get; set; }

        private static IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            return config;
        }

        public KudoTestConfiguration KudoConfiguration { get; set; }
    }
}
