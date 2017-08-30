using iKudo.Domain.Interfaces;
using iKudo.Domain.Logic;
using iKudo.Domain.Model;
using iKudo.Dtos;
using iKudo.Parsers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
                //builder.AddUserSecrets();

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
            
            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            }
            );

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.Audience = Configuration["AppSettings:Auth0:ClientId"];
                o.Authority = Configuration["AppSettings:Auth0:Domain"];
            });

            services.Add(new ServiceDescriptor(typeof(IManageBoards), typeof(BoardManager), ServiceLifetime.Scoped));
            services.Add(new ServiceDescriptor(typeof(IManageJoins), typeof(JoinManager), ServiceLifetime.Scoped));
            services.Add(new ServiceDescriptor(typeof(INotify), typeof(Notifier), ServiceLifetime.Scoped));
            services.Add(new ServiceDescriptor(typeof(IManageKudos), typeof(KudosManager), ServiceLifetime.Scoped));
            services.Add(new ServiceDescriptor(typeof(IManageUsers), typeof(UserManager), ServiceLifetime.Scoped));
            services.AddSingleton(typeof(IProvideTime), typeof(DefaultTimeProvider));
            services.AddScoped(typeof(IUserSearchCriteriaParser), typeof(UserSearchCriteriaParser));
            services.AddScoped(typeof(IKudoSearchCriteriaParser), typeof(KudoSearchCriteriaParser));

            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });
            var mapper = config.CreateMapper();
            //services.AddSingleton(new DefaultDtoFactory(mapper));
            services.Add(new ServiceDescriptor(typeof(IDtoFactory), new DefaultDtoFactory(mapper)));
            //services.AddSingleton(mapper);

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

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute("companyGet", "api/company/{id}");
                routes.MapRoute("joinRequestGet", "api/joinRequest/{id}");
                routes.MapRoute("kudoGet", "api/kudos/{id}");
            });
        }
    }
}
