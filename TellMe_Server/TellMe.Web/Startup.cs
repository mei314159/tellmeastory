using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using TellMe.DAL;
using TellMe.DAL.Contracts;
using TellMe.DAL.Contracts.Repositories;
using TellMe.DAL.Types;
using TellMe.DAL.Types.Repositories;
using TellMe.DAL.Contracts.Services;
using TellMe.DAL.Types.Services;
using TellMe.Web.DTO;
using TellMe.DAL.Types.Domain;
using Microsoft.AspNetCore.Identity;
using Hangfire;
using TellMe.DAL.Types.AzureBlob;
using FluentValidation.AspNetCore;
using System.Reflection;
using Microsoft.AspNetCore.Http.Features;
using TellMe.DAL.Contracts.PushNotifications;
using TellMe.DAL.Types.PushNotifications;
using TellMe.Web.Automapper;

namespace TellMe.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
            Environment = env;
            Configuration = builder.Build();
            AutomapperConfig.Initialize();
        }

        public IHostingEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                b => { }));
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.User.RequireUniqueEmail = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireDigit = true;
                    options.Password.RequiredLength = 4;
                    options.Password.RequireLowercase = false;
                }).AddEntityFrameworkStores<AppDbContext>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            services.AddTransient(typeof(IRepository<,>), typeof(Repository<,>));
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IStoryService, StoryService>();
            services.AddTransient<IStorageService, StorageService>();
            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<ITribeService, TribeService>();
            services.AddTransient<IPushNotificationsService, PushNotificationsService>();
            services.AddTransient<ICommentService, CommentService>();
            services.AddSingleton(Environment);
            services.Configure<Audience>(Configuration.GetSection("Audience"));
            services.Configure<PushSettings>(Configuration.GetSection("Push"));
            services.Configure<AzureBlobSettings>(Configuration.GetSection("AzureBlob"));
            ConfigureJwtAuthService(services);

            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 1048576 * 500; //500 Megabytes
            });
            services.AddHangfire(x => x.UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnection")));
            services.AddMvc().AddFluentValidation(
                fv => fv.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();
            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
            }
            app.UseHangfireServer();
            app.UseAuthentication();
            app.UseMvc();

            RecurringJob.AddOrUpdate<PushFeedbackService>(x => x.CheckExpiredTokens(), Cron.Daily);
        }

        public void ConfigureJwtAuthService(IServiceCollection services)
        {
            var audienceConfig = Configuration.GetSection("Audience");
            var symmetricKeyAsBase64 = audienceConfig["Secret"];
            var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
            var signingKey = new SymmetricSecurityKey(keyByteArray);

            var tokenValidationParameters = new TokenValidationParameters
            {
                // The signing key must match!  
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

                // Validate the JWT Issuer (iss) claim  
                ValidateIssuer = true,
                ValidIssuer = audienceConfig["Iss"],

                // Validate the JWT Audience (aud) claim  
                ValidateAudience = true,
                ValidAudience = audienceConfig["Aud"],

                // Validate the token expiry  
                ValidateLifetime = true,

                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.TokenValidationParameters = tokenValidationParameters;
            });
        }
    }
}
