using Crudspa.Framework.Core.Shared.Extensions;

namespace Crudspa.Framework.Auth.Server.Services;

public class AuthDestinationService(IConfiguration configuration)
{
    private const String Key = "Crudspa.Framework.Auth.Server.DestinationUrlsJson";
    private readonly Dictionary<String, Uri> _destinations = ReadDestinations(configuration);

    public Uri Resolve(String audience, String handoffCode)
    {
        if (!_destinations.TryGetValue(audience, out var destination))
            throw new InvalidOperationException($"Authentication destination '{audience}' is not configured.");

        return new UriBuilder(destination)
        {
            Query = $"code={Uri.EscapeDataString(handoffCode)}",
        }.Uri;
    }

    public Uri ResolveFallback(String audience, String returnPath)
    {
        if (!_destinations.TryGetValue(audience, out var destination))
            throw new InvalidOperationException($"Authentication destination '{audience}' is not configured.");

        return new(new Uri(destination.GetLeftPart(UriPartial.Authority)), returnPath);
    }

    private static Dictionary<String, Uri> ReadDestinations(IConfiguration configuration)
    {
        var json = configuration[Key];
        var configured = json.HasSomething() ? json!.FromJson<Dictionary<String, String>>() : null;

        if (configured is null)
            return new(StringComparer.OrdinalIgnoreCase);

        var destinations = new Dictionary<String, Uri>(StringComparer.OrdinalIgnoreCase);

        foreach (var item in configured)
        {
            if (!Uri.TryCreate(item.Value, UriKind.Absolute, out var uri)
                || !String.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase)
                || uri.UserInfo.HasSomething()
                || uri.Fragment.HasSomething()
                || uri.Query.HasSomething())
                throw new InvalidOperationException($"Authentication destination '{item.Key}' must be an exact HTTPS URL without user information, query, or fragment.");

            destinations.Add(item.Key, uri);
        }

        return destinations;
    }
}