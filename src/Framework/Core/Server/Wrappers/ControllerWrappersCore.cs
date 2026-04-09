using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;

namespace Crudspa.Framework.Core.Server.Wrappers;

public class ControllerWrappersCore(ISessionWrappers sessionWrappers, ILogger<ControllerWrappersCore> logger) : IControllerWrappers
{
    public async Task<ActionResult> RequireSession(HttpRequest request, Func<Session, Task<ActionResult>> func)
    {
        if (!ReadSessionId(request, out var sessionId))
        {
            logger.LogWarning("Invalid or missing SessionId cookie.");
            return new StatusCodeResult(StatusCodes.Status403Forbidden);
        }

        var sessionResponse = await sessionWrappers.RequireSession(new(sessionId), session => Task.FromResult(new Response<Session>(session)));

        if (!sessionResponse.Ok)
        {
            logger.LogWarning("Session not found.");
            return new StatusCodeResult(StatusCodes.Status403Forbidden);
        }

        return await func.Invoke(sessionResponse.Value);
    }

    public async Task<ActionResult> RequirePermission(HttpRequest request, Guid permissionId, Func<Session, Task<ActionResult>> func, [CallerMemberName] String callingMethod = "")
    {
        if (!ReadSessionId(request, out var sessionId))
        {
            logger.LogWarning("Invalid or missing SessionId cookie.");
            return new StatusCodeResult(StatusCodes.Status403Forbidden);
        }

        var permissionResponse = await sessionWrappers.RequirePermission(new(sessionId), permissionId, session => Task.FromResult(new Response<Session>(session)), callingMethod);

        if (!permissionResponse.Ok)
        {
            logger.LogWarning("Permission denied for SessionId: {SessionId}, PermissionId: {PermissionId}", sessionId, permissionId);
            return new StatusCodeResult(StatusCodes.Status403Forbidden);
        }

        return await func.Invoke(permissionResponse.Value);
    }

    private static Boolean ReadSessionId(HttpRequest request, out Guid sessionId)
    {
        sessionId = Guid.Empty;
        var cookie = request.Cookies[Constants.CookieKeys.SessionId];
        return cookie.HasSomething() && Guid.TryParse(cookie, out sessionId);
    }
}