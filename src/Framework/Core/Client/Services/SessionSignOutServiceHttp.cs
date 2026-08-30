namespace Crudspa.Framework.Core.Client.Services;

public class SessionSignOutServiceHttp(HttpClient http)
{
    private const String RequestTokenUrl = "auth/request-token";
    private const String SignOutUrl = "auth/sign-out";

    public async Task<Boolean> SignOut()
    {
        try
        {
            using var tokenResponse = await http.GetAsync(RequestTokenUrl);

            if (!tokenResponse.IsSuccessStatusCode
                || !tokenResponse.Headers.TryGetValues(Constants.HeaderKeys.RequestVerificationToken, out var values))
                return false;

            var token = values.FirstOrDefault();

            if (token.HasNothing())
                return false;

            using var request = new HttpRequestMessage(HttpMethod.Post, SignOutUrl);
            request.Headers.Add(Constants.HeaderKeys.RequestVerificationToken, token);

            using var response = await http.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException)
        {
            return false;
        }
    }
}