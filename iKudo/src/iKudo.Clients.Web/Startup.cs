using iKudo.Clients.Web.Controllers.Api.ModelBinders;
using iKudo.Clients.Web.Filters;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Logic;
using iKudo.Domain.Model;
using iKudo.Dtos;
using iKudo.Parsers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace iKudo.Clients.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();

            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(Configuration).CreateLogger();
            Environment = env;
        }

        public IConfigurationRoot Configuration { get; }

        public IHostingEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<IISOptions>(options =>
            {
                options.ForwardClientCertificate = false;
            });

            services.AddOptions();

            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            services.AddEntityFrameworkSqlServer().AddDbContext<KudoDbContext>(x =>
            {
                x.UseSqlServer(connectionString, b => b.MigrationsAssembly("iKudo.Domain"));
            });

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.Audience = Configuration["AppSettings:Auth0:Audience"];
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

            var kudoCypher = new DefaultKudoCypher(Configuration["AppSettings:KudoCypherPrefix"]);
            services.Add(new ServiceDescriptor(typeof(IKudoCypher), kudoCypher));

            RegisterFileStorage(services);
            RegisterMapper(services);

            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(ValidationFilterAttribute));
                options.ModelBinderProviders.Insert(0, new BoardSearchCriteriaBinderProvider());
                options.ModelBinderProviders.Insert(0, new JoinSearchCriteriaBinderProvider());
                options.ModelBinderProviders.Insert(0, new KudosSearchCriteriaBinderProvider(new KudoSearchCriteriaParser()));
                options.ModelBinderProviders.Insert(0, new NotificationSearchCriteriaBinderProvider());
            })
            .AddJsonOptions(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

            services.AddSingleton<ExceptionHandleAttribute>();

            ISendEmails emailSender = new SendGridEmailSender(Configuration["SendGrid:ApiKey"]);
            services.Add(new ServiceDescriptor(typeof(ISendEmails), emailSender));
            IGenerateBoardInvitationEmail boardInvitationMailGenerator =
                new BoardInvitationEmailGenerator(Configuration["AppSettings:FromEmail"], Configuration["AppSettings:BoardInvitationAcceptUrl"]);
            services.Add(new ServiceDescriptor(typeof(IGenerateBoardInvitationEmail), boardInvitationMailGenerator));
        }

        private static void RegisterMapper(IServiceCollection services)
        {
            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });
            var mapper = config.CreateMapper();
            services.Add(new ServiceDescriptor(typeof(IDtoFactory), new DefaultDtoFactory(mapper)));
        }

        private void RegisterFileStorage(IServiceCollection services)
        {
            string imagesPath = Configuration.GetValue<string>("AppSettings:Paths:KudoImages");
            FileStorage fileStorage = new FileStorage(Environment.WebRootPath, imagesPath);
            services.Add(new ServiceDescriptor(typeof(IFileStorage), fileStorage));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, KudoDbContext dbContext)
        {
            loggerFactory.AddSerilog();

            dbContext.Database.Migrate();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();

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
