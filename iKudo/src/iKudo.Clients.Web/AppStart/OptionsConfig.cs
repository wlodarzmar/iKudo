using iKudo.Clients.Web.Configuration;
using iKudo.Domain.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace iKudo.Clients.Web.AppStart
{
    public static class OptionsConfig
    {
        public static void AddConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<ClientAppConfig>(clientAppConfig =>
            {
                clientAppConfig.InvitationAcceptUrlFormat = configuration["AppSettings:Boards:InvitationAcceptUrlFormat"];
                clientAppConfig.Auth0Config.ReturnUrl = configuration["AppSettings:Auth0:ReturnUrl"];
                clientAppConfig.Auth0Config.ClientId = configuration["AppSettings:Auth0:ClientId"];
                clientAppConfig.Auth0Config.Domain = configuration["AppSettings:Auth0:Domain"];
                clientAppConfig.Auth0Config.Audience = configuration["AppSettings:Auth0:Audience"];
                clientAppConfig.IkudoPageUrl = configuration["AppSettings:IkudoPageUrl"];
            });
            services.Configure<BoardsConfig>(configuration.GetSection("AppSettings:Boards"));
        }
    }
}
