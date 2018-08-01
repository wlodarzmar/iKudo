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
                clientAppConfig.ReturnUrl = configuration["AppSettings:Auth0:ReturnUrl"];
                clientAppConfig.BoardInvitationAcceptUrlFormat = configuration["AppSettings:BoardInvitationAcceptUrlFormat"];
            });
            services.Configure<BoardsConfig>(configuration.GetSection("AppSettings:Boards"));
        }
    }
}
