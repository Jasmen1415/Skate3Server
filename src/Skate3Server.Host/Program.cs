using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Autofac.Extensions.DependencyInjection;
using Bedrock.Framework;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;

namespace Skate3Server.Host
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Setup NLog
            LogManager.Setup().LoadConfigurationFromAppSettings();

            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                .UseNLog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(serverOptions =>
                        {
                            //TODO: 42127 ssl version of gosredirector
                            //gosredirector (Blaze)
                            serverOptions.ListenAnyIP(42100,
                                options =>
                                {
                                    //Force bedrock's connection logger
                                    options.UseConnectionLogging(loggingFormatter: HexLoggingFormatter)
                                        .UseConnectionHandler<BlazeConnectionHandler>();
                                });
                            //eadpgs-blapp001 (Blaze) //TODO: should be ssl
                            serverOptions.ListenAnyIP(10744,
                                options => {
                                    options.UseConnectionLogging(loggingFormatter: HexLoggingFormatter)
                                    .UseConnectionHandler<BlazeConnectionHandler>(); });
                            //gostelemetry //TODO: no idea what format this is in
                            //serverOptions.ListenAnyIP(9946,
                            //    options => { options.UseConnectionHandler<BlazeConnectionHandler>(); });
                            //qos servers [gosgvaprod-qos01, gosiadprod-qos01, gossjcprod-qos01] (HTTP)
                            serverOptions.ListenAnyIP(17502);
                            //TODO qos UDP 17499
                            //downloads.skate.online (HTTP)
                            serverOptions.ListenAnyIP(80);
                        })
                        .UseStartup<Startup>();
                });
        }

        private static void HexLoggingFormatter(Microsoft.Extensions.Logging.ILogger logger, string method, ReadOnlySpan<byte> buffer)
        {
            if (!logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Trace))
                return;

            var builder = new StringBuilder($"{method}[{buffer.Length}] ");

            // Write the hex
            foreach (var b in buffer)
            {
                builder.Append(b.ToString("X2"));
                builder.Append(" ");
            }

            logger.LogTrace(builder.ToString());
        }
    }
}
