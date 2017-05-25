using iKudo.Domain.Interfaces;
using iKudo.Domain.Logic;
using iKudo.Domain.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;

namespace iKudo.Clients.Web
{
    public class Startup
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();

                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.

            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddOptions();

            //Add application services.

            string connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddEntityFrameworkSqlServer().AddDbContext<KudoDbContext>(x =>
            {
                x.UseSqlServer(connectionString, b => b.MigrationsAssembly("iKudo.Domain"));
            });

            services.Add(new ServiceDescriptor(typeof(IBoardManager), typeof(BoardManager), ServiceLifetime.Transient));
            services.Add(new ServiceDescriptor(typeof(IManageJoins), typeof(JoinManager), ServiceLifetime.Transient));
            services.AddSingleton(typeof(ITimeProvider), typeof(DefaultTimeProvider));

            services.AddMvc().AddJsonOptions(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715
            var options = new JwtBearerOptions
            {
                Audience = Configuration["AppSettings:Auth0:ClientId"],
                Authority = Configuration["AppSettings:Auth0:Domain"]
            };

            app.UseJwtBearerAuthentication(options);

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute("companyGet", "api/company/{id}");
                routes.MapRoute("joinRequestGet", "api/joinRequest/{id}");
            });
        }
    }
}
