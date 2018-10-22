using Microsoft.Extensions.Configuration;

namespace iKudo.Clients.Web.UITests
{
    public class BaseTest
    {
        public BaseTest()
        {
            Configuration = InitConfiguration();
        }

        public static IConfiguration Configuration { get; private set; }

        private static IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            return config;
        }
    }
}
