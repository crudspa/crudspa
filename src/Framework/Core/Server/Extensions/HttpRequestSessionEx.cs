namespace Crudspa.Framework.Core.Server.Extensions;

public static class HttpRequestSessionEx
{
    extension(HttpRequest request)
    {
        public Boolean TryReadSessionId(out Guid sessionId)
        {
            var authenticatedSessionId = request.HttpContext.User.ReadAuthenticatedSessionId();

            if (authenticatedSessionId is not null)
            {
                sessionId = authenticatedSessionId.Value;
                return true;
            }

            sessionId = Guid.Empty;
            var key = Constants.CookieKeys.Resolve(Constants.CookieKeys.SessionId, request.Host.Host, request.Host.Port);
            var cookie = request.Cookies[key];
            return cookie.HasSomething() && Guid.TryParse(cookie, out sessionId);
        }
    }
}