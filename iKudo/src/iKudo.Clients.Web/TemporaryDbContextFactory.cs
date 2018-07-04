using iKudo.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;

namespace iKudo.Clients.Web
{
    //TODO: move this to domain
    public class TemporaryDbContextFactory : IDesignTimeDbContextFactory<KudoDbContext>
    {
        public KudoDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<KudoDbContext>();
            IConfigurationRoot configuration = new ConfigurationBuilder()
              .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
              .AddJsonFile("appsettings.json")
              .AddUserSecrets<Startup>()
              .Build();

            builder.UseSqlServer(configuration["ConnectionStrings:DefaultConnection"]);
            return new KudoDbContext(builder.Options);
        }
    }
}
