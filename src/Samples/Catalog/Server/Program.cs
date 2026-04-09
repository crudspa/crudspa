using Crudspa.Framework.Core.Server.Controllers;
using Crudspa.Framework.Core.Server.Filters;
using Crudspa.Framework.Core.Shared.Services;
using Crudspa.Samples.Catalog.Server.Hubs;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Azure.SignalR;
using Serilog;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.MsSqlServer.Destructurers;
using System.Diagnostics;
using System.IO.Compression;
using System.Text.Json.Serialization;

namespace Crudspa.Samples.Catalog.Server;

public class Program
{
    public static async Task Main(String[] args)
    {
        const String outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj} {NewLine}{Exception}";
#if DEBUG
        const String debugLogPath = @"c:\data\temp\logs\samples-catalog-.txt";
        Directory.CreateDirectory(@"c:\data\temp\logs");
#endif

        Log.Logger = new LoggerConfiguration()
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

            var configuration = builder.Configuration;
            var configService = new ServerConfigServiceCore(configuration);
            var config = configService.Fetch();

            if (config.AppInsightsConnection.HasSomething())
                builder.Services.AddApplicationInsightsTelemetry(options => options.ConnectionString = config.AppInsightsConnection);

            builder.Host.UseSerilog((context, services, serilogConfig) =>
            {
                serilogConfig
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

                if (config.AppInsightsConnection.HasSomething())
                    serilogConfig.WriteTo.ApplicationInsights(services.GetRequiredService<TelemetryClient>(), TelemetryConverter.Traces);
            });

            builder.Services.AddHttpContextAccessor();

            Registry.RegisterServices(builder.Services, config);

            builder.Services.AddMemoryCache(options =>
            {
                options.SizeLimit = 50000;
            });

            builder.Services.AddRazorPages(options =>
            {
                options.Conventions.Add(new NoCachePageConvention());
                options.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());
            });

            var mvcBuilder = builder.Services.AddControllers(options =>
                {
                    options.Filters.Add(new IgnoreAntiforgeryTokenAttribute());
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });

            mvcBuilder.PartManager.ApplicationParts.Add(new AssemblyPart(typeof(AudioFileController).Assembly));

            var signalR = builder.Services.AddSignalR(options =>
                {
                    options.MaximumReceiveMessageSize = Int64.MaxValue;
                    options.EnableDetailedErrors = !config.SignalRUseAzure;
                    options.MaximumParallelInvocationsPerClient = 8;
                })
                .AddJsonProtocol(options =>
                {
                    options.PayloadSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });

            if (config.SignalRUseAzure)
            {
                signalR.AddAzureSignalR(options =>
                {
                    options.AccessTokenLifetime = TimeSpan.FromHours(6);
                    options.ApplicationName = config.SignalRAppName;
                    options.ServerStickyMode = ServerStickyMode.Preferred;
                    options.GracefulShutdown.Mode = GracefulShutdownMode.MigrateClients;
                    options.GracefulShutdown.Timeout = TimeSpan.FromMinutes(1);
                });
            }

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.Limits.MaxRequestBodySize = 4L * 1024L * 1024L * 1024L;
                options.Limits.MinRequestBodyDataRate = null;
                options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(30);
                options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(1);
            });

            builder.Services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = 4L * 1024L * 1024L * 1024L;
            });

            builder.Services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat([
                    "application/font-sfnt",
                    "application/font-woff",
                    "application/font-woff2",
                ]);
            });

            builder.Services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            });

            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 4L * 1024L * 1024L * 1024L;
                options.ValueLengthLimit = Int32.MaxValue;
                options.MultipartHeadersLengthLimit = Int32.MaxValue;
            });

            var webApp = builder.Build();

            if (webApp.Environment.IsDevelopment())
            {
                webApp.UseWebAssemblyDebugging();
            }
            else
                webApp.UseHsts();

            webApp.UseBlazorFrameworkFiles();
            webApp.UseHttpsRedirection();
            webApp.UseResponseCompression();
            webApp.UseRouting();
            webApp.UseSerilogRequestLogging();
            webApp.UseStaticFiles();

            webApp.Use(async (context, next) =>
            {
                context.Response.Headers.Append("X-Frame-Options", "SAMEORIGIN");
                await next();
            });

            webApp.MapHub<CatalogHub>("/hub");

            webApp.MapWhen(context => context.Request.Path.StartsWithSegments("/api"), webAppConfig =>
                webAppConfig.UseEndpoints(endpoints => endpoints.MapControllers()));

            webApp.MapWhen(context => !context.Request.Path.StartsWithSegments("/api"), webAppConfig =>
                webAppConfig.UseEndpoints(endpoints => endpoints.MapFallbackToPage("/Catalog")));

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