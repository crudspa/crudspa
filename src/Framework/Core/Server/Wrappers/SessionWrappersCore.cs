namespace Crudspa.Framework.Core.Server.Wrappers;

public class SessionWrappersCore(ISessionFetcher sessionFetcher, IAccessDeniedService accessDeniedService)
    : ISessionWrappers
{
    public async Task<Response> RequireSession(Request request, Func<Session, Task<Response>> func)
    {
        var session = await sessionFetcher.Fetch(request.SessionId);

        if (session is null)
            return new(Constants.ErrorMessages.PermissionDenied);

        return await func.Invoke(session);
    }

    public async Task<Response<T>> RequireSession<T>(Request request, Func<Session, Task<Response<T>>> func)
        where T : class?
    {
        var session = await sessionFetcher.Fetch(request.SessionId);

        if (session is null)
            return new(Constants.ErrorMessages.PermissionDenied);

        return await func.Invoke(session);
    }

    public async Task<Response> RequireUser(Request request, Func<Session, Task<Response>> func)
    {
        var session = await sessionFetcher.Fetch(request.SessionId);

        if (session?.User?.Id is null)
            return new(Constants.ErrorMessages.PermissionDenied);

        return await func.Invoke(session);
    }

    public async Task<Response<T>> RequireUser<T>(Request request, Func<Session, Task<Response<T>>> func)
        where T : class?
    {
        var session = await sessionFetcher.Fetch(request.SessionId);

        if (session?.User?.Id is null)
            return new(Constants.ErrorMessages.PermissionDenied);

        return await func.Invoke(session);
    }

    public async Task<Response> RequirePermission(Request request, Guid permissionId, Func<Session, Task<Response>> func, String callingMethodName = "")
    {
        return await RequireSession(request, async session =>
        {
            if (!session.Permissions.HasAny(x => x.Equals(permissionId)))
            {
                await accessDeniedService.Add(new()
                {
                    SessionId = request.SessionId,
                    EventType = Constants.AccessDeniedEventTypes.PermissionDenied,
                    PermissionId = permissionId,
                    Method = callingMethodName,
                });

                return new(Constants.ErrorMessages.PermissionDenied);
            }

            return await func.Invoke(session);
        });
    }

    public async Task<Response<T>> RequirePermission<T>(Request request, Guid permissionId, Func<Session, Task<Response<T>>> func, String callingMethodName = "")
        where T : class?
    {
        return await RequireSession<T>(request, async session =>
        {
            if (!session.Permissions.HasAny(x => x.Equals(permissionId)))
            {
                await accessDeniedService.Add(new()
                {
                    SessionId = request.SessionId,
                    EventType = Constants.AccessDeniedEventTypes.PermissionDenied,
                    PermissionId = permissionId,
                    Method = callingMethodName,
                });

                return new(Constants.ErrorMessages.PermissionDenied);
            }

            return await func.Invoke(session);
        });
    }
}