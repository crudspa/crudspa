using Crudspa.Framework.Core.Server.Services;
using Crudspa.Framework.Jobs.Server.Hosts;
using Microsoft.ApplicationInsights;
using Serilog;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.MsSqlServer.Destructurers;
using Serilog.Sinks.ApplicationInsights.TelemetryConverters;

namespace Crudspa.Samples.Jobs.Engine;

public class Program
{
    public static async Task Main(String[] args)
    {
        const String outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj} {NewLine}{Exception}";

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: outputTemplate)
            .WriteTo.File(@"c:\data\temp\logs\jobs-.txt",
                rollingInterval: RollingInterval.Day,
                fileSizeLimitBytes: 200L * 1024L * 1024L,
                retainedFileCountLimit: 30,
                outputTemplate: outputTemplate)
            .CreateBootstrapLogger();

        Directory.SetCurrentDirectory(AppContext.BaseDirectory);

        try
        {
            var builder = Host.CreateDefaultBuilder(args);
#if DEBUG
            builder.UseEnvironment("Development");
#endif
            builder.UseSerilog((hostContext, services, serilogConfig) =>
            {
                var configuration = hostContext.Configuration;
                var config = new ServerConfigServiceCore(configuration).Fetch();

                serilogConfig
                    .ReadFrom.Services(services)
                    .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
                        .WithDefaultDestructurers()
                        .WithDestructurers([new SqlExceptionDestructurer()]))
                    .Enrich.FromLogContext()
                    .WriteTo.Console(outputTemplate: outputTemplate)
                    .WriteTo.File(@"c:\data\temp\logs\jobs-.txt",
                        rollingInterval: RollingInterval.Day,
                        fileSizeLimitBytes: 200L * 1024L * 1024L,
                        retainedFileCountLimit: 30,
                        outputTemplate: outputTemplate);

                if (config.AppInsightsConnection.HasSomething())
                    serilogConfig.WriteTo.ApplicationInsights(services.GetRequiredService<TelemetryClient>(), new TraceTelemetryConverter());
            });

            builder.ConfigureLogging((_, loggingBuilder) =>
            {
                loggingBuilder.SetMinimumLevel(LogLevel.Debug);
            });

            builder.ConfigureServices((hostContext, services) =>
            {
                var configuration = hostContext.Configuration;
                var config = new ServerConfigServiceCore(configuration).Fetch();

                if (config.AppInsightsConnection.HasSomething())
                    services.AddApplicationInsightsTelemetryWorkerService(options => options.ConnectionString = config.AppInsightsConnection);

                services.AddHttpContextAccessor();

                Registry.RegisterServices(services, config);

                services.AddHostedService<Scheduler>();
                services.AddHostedService<Worker>();
            }).UseWindowsService();

            var host = builder.Build();
            await host.RunAsync();
        }
        catch (Exception ex)
        {
            if (Debugger.IsAttached) Debugger.Break();
            Log.Fatal(ex, "Unhandled exception in Program.Main of the jobs engine.");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
}