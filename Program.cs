using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
                    webBuilder.UseStartup<Startup> ();
                    //.UseUrls ("https://localhost:4000");
                });
        }
    }
}