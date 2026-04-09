using Azure;
using Azure.Messaging.EventGrid;
using System.Text;

namespace Crudspa.Framework.Core.Server.Services;

public class GatewayServiceEventGrid : IGatewayService
{
    private readonly ILogger<GatewayServiceEventGrid> _logger;
    private readonly EventGridPublisherClient? _client;
    private readonly IReadOnlyList<Uri> _receiverUrls;
    private static readonly HttpClient HttpClient = new() { Timeout = TimeSpan.FromSeconds(10) };
    private const String Version = "1.0";

    public GatewayServiceEventGrid(IServerConfigService configService, ILogger<GatewayServiceEventGrid> logger)
    {
        _logger = logger;
        var config = configService.Fetch();
        _receiverUrls = ParseReceiverUrls(config.EventReceiverUrls);

        if (config.EventTopicKey.HasSomething() && config.EventTopicEndpoint.HasSomething())
        {
            var credential = new AzureKeyCredential(config.EventTopicKey);
            _client = new(new(config.EventTopicEndpoint), credential);
        }
        else if (_receiverUrls.IsEmpty())
            _logger.LogWarning("Config did not have values for EventTopicKey, EventTopicEndpoint, or EventReceiverUrls.");
    }

    public Task Publish<T>(T eventObject) where T : class
    {
        _ = PublishCore(eventObject);
        return Task.CompletedTask;
    }

    private async Task PublishCore<T>(T eventObject) where T : class
    {
        var subject = $"{typeof(T).Namespace}.{typeof(T).Name}";

        try
        {
            var gridEvent = new EventGridEvent(subject, typeof(T).Name, Version, eventObject);

            if (_client is not null)
            {
                await PublishToEventGrid(gridEvent);
                return;
            }

            if (await PublishToReceivers(gridEvent))
                return;

            _logger.LogWarning("No event publisher accepted the event '{subject}'.", subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected failure while publishing event '{subject}'.", subject);
        }
    }

    private async Task PublishToEventGrid(EventGridEvent gridEvent)
    {
        try
        {
            await _client!.SendEventAsync(gridEvent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Publication failed for event '{subject}'.", gridEvent.Subject);
        }
    }

    private async Task<Boolean> PublishToReceivers(EventGridEvent gridEvent)
    {
        if (_receiverUrls.IsEmpty())
            return false;

        var payload = new[] { gridEvent }.ToJson();
        if (payload.HasNothing())
            return false;

        var results = await Task.WhenAll(_receiverUrls.Select(uri => PublishToReceiver(uri, payload!, gridEvent.Subject)));
        return results.Any(x => x);
    }

    private async Task<Boolean> PublishToReceiver(Uri uri, String payload, String subject)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = new StringContent(payload, Encoding.UTF8, "application/json"),
            };

            using var response = await HttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
                return true;

            _logger.LogWarning("Local event receiver '{uri}' returned status {statusCode} for event '{subject}'.", uri, (Int32)response.StatusCode, subject);
        }
        catch (TaskCanceledException)
        {
            _logger.LogDebug("Timed out contacting local event receiver '{uri}' for event '{subject}'.", uri, subject);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogDebug("Could not reach local event receiver '{uri}' for event '{subject}'. {message}", uri, subject, ex.Message);
        }

        return false;
    }

    private static List<Uri> ParseReceiverUrls(String receiverUrls)
    {
        return receiverUrls
            .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(url => Uri.TryCreate(url, UriKind.Absolute, out var uri) ? uri : null)
            .OfType<Uri>()
            .ToList();
    }
}