using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Debugging;

namespace zero_alloc.webapi
{
    public static class LoggingConfig
    {
        public static IWebHostBuilder UseAppLogging(this IWebHostBuilder webHostBuilder)
        {
            return webHostBuilder
                .UseSerilog((context, configuration) =>
                    {
                        configuration
                            .Enrich.FromLogContext()
                            .Enrich.WithMachineName()
                            .ReadFrom.Configuration(context.Configuration);
                    }
                );
        }

        public static void FlushLogsOnShutdown(this IApplicationLifetime lifetime)
        {
            lifetime.ApplicationStopped.Register(() =>
                {
                    Log.CloseAndFlush();
                    SelfLog.Disable();
                }
            );
        }
    }
}