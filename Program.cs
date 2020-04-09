using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ChatServer {
    public class Program {
        public static void Main (string[] args) {
            CreateHostBuilder (args).Build ().Run ();
        }

        public static IHostBuilder CreateHostBuilder (string[] args) {
            // var host = new WebHostBuilder()
            //     .UseKestrel()
            //     .UseContentRoot(Directory.GetCurrentDirectory())
            //     .UseUrls("http://localhost:5000", "httpS://localhost:5001")
            //     .UseIISIntegration()
            //     .UseStartup<Startup>();
            return Host.CreateDefaultBuilder (args)
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            })
            .ConfigureWebHostDefaults (webBuilder => {
                webBuilder.UseStartup<Startup> ();
                    //.UseUrls ("https://localhost:4000");
            });
            //return host;
        }
    }
}