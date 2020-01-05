using System;
using System.Net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Web;
using Sentry;

namespace MyApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            #region Sentry/NLog

            //We used NLog.Targets.Sentry2 library formerly
            //But it was not based on NetStandard and used an unstable SharpRaven library
            //So we decided to replace it with a better library
            //The NLog.Targets.Sentry3 library supports NetStandard2.0 and uses an updated version of SharpRaven library.
            //But Sentry.NLog is the official sentry library integrated with nlog and better than all others.

            //NLog.Targets.Sentry3
            //https://github.com/CurtisInstruments/NLog.Targets.Sentry

            //Sentry SDK for .NET
            //https://github.com/getsentry/sentry-dotnet

            //Sample integration of NLog with Sentry
            //https://github.com/getsentry/sentry-dotnet/tree/master/samples/Sentry.Samples.NLog


            //Set deafult proxy
            WebRequest.DefaultWebProxy = new WebProxy("http://127.0.0.1:8118", true) { UseDefaultCredentials = true };

            // You can configure your logger using a configuration file:

            // If using an NLog.config xml file, NLog will load the configuration automatically Or, if using a
            // different file, you can call the following to load it for you: 
            //LogManager.Configuration = LogManager.LoadConfiguration("NLog-file.config").Configuration;

            var logger = LogManager.GetCurrentClassLogger();

            // Or you can configure it with code:
            //UsingCodeConfiguration();

            #endregion

            try
            {
                logger.Debug("init main");
                CreateWebHostBuilder(args).Build().Run();
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
                LogManager.Flush();
                LogManager.Shutdown();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging(options => options.ClearProviders())
                .UseNLog()
                .UseStartup<Startup>();

        private static void UsingCodeConfiguration()
        {
            // Other overloads exist, for example, configure the SDK with only the DSN or no parameters at all.
            var config = new LoggingConfiguration();

            config.AddSentry(options =>
            {
                options.Layout = "${message}";
                options.BreadcrumbLayout = "${logger}: ${message}"; // Optionally specify a separate format for breadcrumbs

                options.MinimumBreadcrumbLevel = NLog.LogLevel.Debug; // Debug and higher are stored as breadcrumbs (default is Info)
                options.MinimumEventLevel = NLog.LogLevel.Error; // Error and higher is sent as event (default is Error)

                // If DSN is not set, the SDK will look for an environment variable called SENTRY_DSN. If
                // nothing is found, SDK is disabled.
                options.Dsn = new Dsn("https://a48f67497c814561aca2c66fa5ee37fc:a5af1a051d6f4f09bdd82472d5c2629d@sentry.io/1340240");

                options.AttachStacktrace = true;
                options.SendDefaultPii = true; // Send Personal Identifiable information like the username of the user logged in to the device

                options.IncludeEventDataOnBreadcrumbs = true; // Optionally include event properties with breadcrumbs
                options.ShutdownTimeoutSeconds = 5;

                options.AddTag("logger", "${logger}");  // Send the logger name as a tag

                options.HttpProxy = new WebProxy("http://127.0.0.1:8118", true) { UseDefaultCredentials = true };
                // Other configuration
            });

            config.AddTarget(new DebuggerTarget("Debugger"));
            config.AddTarget(new ColoredConsoleTarget("Console"));

            config.AddRuleForAllLevels("Console");
            config.AddRuleForAllLevels("Debugger");

            LogManager.Configuration = config;
        }
    }
}
