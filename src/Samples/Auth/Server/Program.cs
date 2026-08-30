using Crudspa.Framework.Auth.Server.Controllers;
using Crudspa.Framework.Core.Shared.Extensions;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.MsSqlServer.Destructurers;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Crudspa.Samples.Auth.Server;

public class Program
{
    public static async Task Main(String[] args)
    {
        const String outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj} {NewLine}{Exception}";
#if DEBUG
        const String debugLogPath = @"c:\data\temp\logs\portals-auth-.txt";
        Directory.CreateDirectory(@"c:\data\temp\logs");
#endif

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft.AspNetCore.Hosting.Diagnostics", Serilog.Events.LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: outputTemplate)
#if DEBUG
            .WriteTo.File(debugLogPath,
                rollingInterval: RollingInterval.Day,
                fileSizeLimitBytes: 200L * 1024L * 1024L,
                retainedFileCountLimit: 30,
                outputTemplate: outputTemplate)
#endif
            .CreateBootstrapLogger();

        try
        {
            var builder = WebApplication.CreateBuilder(args);
            var appInsightsConnection = builder.Configuration["Crudspa.Framework.Core.Server.AppInsightsConnection"];
            var buildNumber = builder.Configuration["Crudspa.Framework.Core.Server.BuildNumber"] ?? "unknown";

            if (appInsightsConnection.HasSomething())
                builder.Services.AddApplicationInsightsTelemetry(options => options.ConnectionString = appInsightsConnection);

            builder.Host.UseSerilog((context, services, serilogConfig) =>
            {
                serilogConfig
                    .MinimumLevel.Override("Microsoft.AspNetCore.Hosting.Diagnostics", Serilog.Events.LogEventLevel.Warning)
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
                        .WithDefaultDestructurers()
                        .WithDestructurers([new SqlExceptionDestructurer()]))
                    .Enrich.FromLogContext()
                    .WriteTo.Console(outputTemplate: outputTemplate);

#if DEBUG
                serilogConfig.WriteTo.File(debugLogPath,
                    rollingInterval: RollingInterval.Day,
                    fileSizeLimitBytes: 200L * 1024L * 1024L,
                    retainedFileCountLimit: 30,
                    outputTemplate: outputTemplate);
#endif

                if (appInsightsConnection.HasSomething())
                    serilogConfig.WriteTo.ApplicationInsights(services.GetRequiredService<TelemetryClient>(), TelemetryConverter.Traces);
            });

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddAuthentication();
            builder.Services.AddAuthorization();
            var mvcBuilder = builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });

            foreach (var part in mvcBuilder.PartManager.ApplicationParts.Where(x => x.Name == typeof(PortalAuthController).Assembly.GetName().Name).ToList())
                mvcBuilder.PartManager.ApplicationParts.Remove(part);

            Registry.RegisterServices(builder.Services, builder.Configuration);

            var webApp = builder.Build();

            if (!webApp.Environment.IsDevelopment())
                webApp.UseHsts();

            webApp.UseHttpsRedirection();
            webApp.UseRouting();
            webApp.UseSerilogRequestLogging();

            webApp.Use(async (context, next) =>
            {
                context.Response.Headers.Append("Cache-Control", "no-store");
                context.Response.Headers.Append("Content-Security-Policy", "default-src 'none'; base-uri 'none'; form-action 'self'; frame-ancestors 'none'");
                context.Response.Headers.Append("Referrer-Policy", "no-referrer");
                context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Append("X-Frame-Options", "DENY");
                await next();
            });

            webApp.UseAuthentication();
            webApp.UseAuthorization();

            webApp.MapGet("/health", () => Results.Ok(new
            {
                Status = "Healthy",
                Build = buildNumber,
            }));
            webApp.MapControllers();

            await webApp.RunAsync();
        }
        catch (Exception ex)
        {
            if (Debugger.IsAttached) Debugger.Break();
            Log.Fatal(ex, "Program could not be started.");
            throw;
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
}