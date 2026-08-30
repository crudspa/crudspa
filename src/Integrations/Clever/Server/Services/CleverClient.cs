using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Crudspa.Integrations.Clever.Server.Contracts.Data;

namespace Crudspa.Integrations.Clever.Server.Services;

public class CleverClient(HttpClient httpClient, CleverConfig config)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public async Task<String> FetchDistrictToken(String districtId, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get,
            $"https://clever.com/oauth/tokens?district={Uri.EscapeDataString(districtId)}");
        Authenticate(request);

        using var response = await Send(request, cancellationToken);
        using var document = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(cancellationToken), cancellationToken: cancellationToken);
        return ReadToken(document.RootElement)
            ?? throw new InvalidOperationException($"Clever did not return an active token for district '{districtId}'.");
    }

    public async Task<JsonDocument> Redeem(String code, String redirectUri, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "https://clever.com/oauth/tokens");
        Authenticate(request);
        request.Content = new FormUrlEncodedContent(new Dictionary<String, String>
        {
            ["code"] = code,
            ["grant_type"] = "authorization_code",
            ["redirect_uri"] = redirectUri,
        });

        using var response = await httpClient.SendAsync(request, cancellationToken);
        var result = JsonDocument.Parse(await response.Content.ReadAsStringAsync(cancellationToken));
        if (!response.IsSuccessStatusCode)
        {
            var error = result.RootElement.TryGetProperty("error", out var value) ? value.GetString() : null;
            result.Dispose();
            throw new AuthenticationFailureException(error ?? $"Clever token exchange returned HTTP {(Int32)response.StatusCode}.");
        }

        return result;
    }

    internal async Task<CleverIdentity> FetchIdentity(String token, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://api.clever.com/userinfo");
        request.Headers.Authorization = new("Bearer", token);

        using var response = await Send(request, cancellationToken);
        return await JsonSerializer.DeserializeAsync<CleverIdentity>(
            await response.Content.ReadAsStreamAsync(cancellationToken), JsonOptions, cancellationToken)
            ?? new();
    }

    public async IAsyncEnumerable<IList<T>> Fetch<T>(
        String path,
        String token,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var next = path.Contains('?') ? $"{path}&limit=10000" : $"{path}?limit=10000";

        while (next is not null)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, new Uri(new Uri("https://api.clever.com"), next));
            request.Headers.Authorization = new("Bearer", token);

            using var response = await Send(request, cancellationToken);
            using var document = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(cancellationToken), cancellationToken: cancellationToken);
            var root = document.RootElement;
            var values = root.GetProperty("data")
                .EnumerateArray()
                .Select(x => x.TryGetProperty("data", out var value) ? value : x)
                .Select(x => x.Deserialize<T>(JsonOptions)!)
                .ToList();

            yield return values;
            next = ReadNext(root);
        }
    }

    private async Task<HttpResponseMessage> Send(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        if (response.IsSuccessStatusCode)
            return response;

        var status = (Int32)response.StatusCode;
        response.Dispose();
        throw new HttpRequestException($"Clever returned HTTP {status} for {request.RequestUri?.AbsolutePath}.", null, response.StatusCode);
    }

    private void Authenticate(HttpRequestMessage request)
    {
        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{config.ClientId}:{config.ClientSecret}"));
        request.Headers.Authorization = new("Basic", credentials);
    }

    private static String? ReadNext(JsonElement root)
    {
        if (!root.TryGetProperty("links", out var links))
            return null;

        foreach (var link in links.EnumerateArray())
            if (link.TryGetProperty("rel", out var rel)
                && rel.GetString() == "next"
                && link.TryGetProperty("uri", out var uri))
                return uri.GetString();

        return null;
    }

    private static String? ReadToken(JsonElement value)
    {
        if (value.ValueKind == JsonValueKind.Array)
            return value.EnumerateArray().Select(ReadToken).FirstOrDefault(x => x is not null);

        if (value.ValueKind != JsonValueKind.Object)
            return null;

        foreach (var name in new[] { "token", "access_token" })
            if (value.TryGetProperty(name, out var token) && token.ValueKind == JsonValueKind.String)
                return token.GetString();

        return value.EnumerateObject().Select(x => ReadToken(x.Value)).FirstOrDefault(x => x is not null);
    }
}