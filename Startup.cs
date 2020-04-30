using System.Text;
using System.Threading.Tasks;
using ChatServer.Helper;
using ChatServer.Hub;
using ChatServer.Repository;
using ChatServer.Repository.Contract;
using ChatServer.Service;
using ChatServer.Service.Contract;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ChatServer {
    public class Startup {
        readonly string MyCustomAllowedOrigins = "_myCustomAllowedOrigins";
        public Startup (IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services) {
            services.Configure<DatabaseSettings> (
                Configuration.GetSection (nameof (DatabaseSettings)));

            services.AddSingleton<IDatabaseSettings> (sp =>
                sp.GetRequiredService<IOptions<DatabaseSettings>> ().Value);

            var appSettingsSection = Configuration.GetSection ("AppSettings");
            services.Configure<AppSettings> (appSettingsSection);

            services.AddSingleton<IAppSettings> (sp =>
                sp.GetRequiredService<IOptions<AppSettings>> ().Value);

            services.AddHttpContextAccessor();

            services.AddCors (options => {
                options.AddPolicy (MyCustomAllowedOrigins,
                    builder => {
                        builder.WithOrigins ("http://localhost:3000")
                            .AllowAnyHeader ()
                            .WithMethods ("GET", "POST")
                            .AllowCredentials ();
                    });
            });

            services.AddControllers ();
            services.AddSignalR ();

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings> ();
            var key = Encoding.ASCII.GetBytes (appSettings.secret);
            services.AddAuthentication (x => {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer (x => {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey (key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                    x.Events = new JwtBearerEvents {
                        OnMessageReceived = context => {
                            var accessToken = context.Request.Query["access_token"];

                            // If the request is for our hub...
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty (accessToken) &&
                                (path.StartsWithSegments ("/chatHub"))) {
                                // Read the token out of the query string
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddSingleton<IApplicationRepository, ApplicationRepository> ();
            services.AddSingleton<IUserRepository, UserRepository> ();
            services.AddSingleton<IFileRepository, FileRepository> ();
            services.AddSingleton<IChatRepository, ChatRepository> ();

            services.AddSingleton<IHubService, HubService> ();
            services.AddSingleton<IAuthenticationService, AuthenticationService> ();
            services.AddSingleton<IChatService, ChatService> ();
            services.AddSingleton<IUserService, UserService> ();
            services.AddSingleton<IApplicationService, ApplicationService> ();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IWebHostEnvironment env, IApplicationService applicationService) {
            if (env.IsDevelopment ()) {
                app.UseDeveloperExceptionPage ();
            } else {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts ();
            }

            app.UseForwardedHeaders (new ForwardedHeadersOptions {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            applicationService.SeedData ();

            app.UseDefaultFiles ();
            app.UseStaticFiles ();

            app.UseHttpsRedirection ();

            app.UseRouting ();

            app.UseCors (MyCustomAllowedOrigins);

            app.UseAuthentication ();
            app.UseAuthorization ();

            app.UseClaimChecking ();

            app.UseEndpoints (endpoints => {
                endpoints.MapHub<ChatHub> ("/chatHub");
                endpoints.MapControllers ();
            });
        }
    }
}