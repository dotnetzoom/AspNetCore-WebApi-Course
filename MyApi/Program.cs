using System;
using System.IO;
using System.Threading.Tasks;
using AspNetCoreRateLimit;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace MyApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //Set default proxy
            //WebRequest.DefaultWebProxy = new WebProxy("http://127.0.0.1:8118", true) { UseDefaultCredentials = true };

            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Debug("init main");

                var webHost = CreateHostBuilder(args).Build();

                using var scope = webHost.Services.CreateScope();

                var clientPolicyStore = scope.ServiceProvider.GetRequiredService<IClientPolicyStore>();

                await clientPolicyStore.SeedAsync();

                var ipPolicyStore = scope.ServiceProvider.GetRequiredService<IIpPolicyStore>();

                await ipPolicyStore.SeedAsync();

                await webHost.RunAsync();
            }
            catch (Exception ex)
            {
                //NLog: catch setup errors
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureLogging(options => options.ClearProviders())
                .ConfigureLogging(logger => {
                    logger.AddFilter(DbLoggerCategory.Database.Command.Name, LogLevel.Information);
                    logger.ClearProviders();
                    logger.SetMinimumLevel(LogLevel.Trace);
                })
                .UseNLog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseContentRoot(Directory.GetCurrentDirectory())
                        .UseIISIntegration()
                        //use in cmd mode, not iis express
                        //.UseKestrel(c => c.AddServerHeader = false)
                        .UseStartup<Startup>();
                });
    }
}
