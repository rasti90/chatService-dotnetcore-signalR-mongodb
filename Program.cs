using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sentry.Extensibility;

namespace ChatServer {
    public class Program {
        public static void Main (string[] args) {
            CreateHostBuilder (args).Build ().Run ();
        }

        public static IHostBuilder CreateHostBuilder (string[] args) {
            return Host.CreateDefaultBuilder (args)
                .ConfigureLogging (logging => {
                    logging.ClearProviders ();
                    logging.AddConsole ();
                })
                .ConfigureWebHostDefaults (webBuilder => {
                    // webBuilder.UseSentry(options =>
                    // {
                    //     options.Debug = true;
                    //     options.MaxRequestBodySize = RequestSize.Always;
                    //     options.Dsn = "Your Sentry Dsn Address";
                    //     options.BeforeSend = @event =>
                    //     {
                    //         // Never report server names
                    //         @event.ServerName = null;
                    //         return @event;
                    //     };
                    // });
                    webBuilder.UseStartup<Startup> ();
                    //.UseUrls ("https://localhost:4000");
                });
        }
    }
}