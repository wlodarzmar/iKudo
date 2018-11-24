using iKudo.Domain.Interfaces;
using Microsoft.AspNetCore.Hosting;

namespace iKudo.Clients.Web.AppStart
{
    public class KudoHostingEnvironment : IEnvironment
    {
        private readonly IHostingEnvironment environment;

        public KudoHostingEnvironment(IHostingEnvironment environment)
        {
            this.environment = environment;
        }

        public string RootPath => environment.WebRootPath;
    }
}
