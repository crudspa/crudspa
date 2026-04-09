using System.Runtime.CompilerServices;

namespace Crudspa.Framework.Core.Server.Wrappers;

public class HubWrappersCore(ILogger<HubWrappersCore> logger, ISessionWrappers sessionWrappers)
    : IHubWrappers
{
    private const String HubMethodFailed = "Unexpected hub error.";

    public async Task<Response> AllowAnonymous(Request request, Func<Task<Response>> func)
    {
        try
        {
            return await func.Invoke();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "SessionId: {SessionId}", request.SessionId);
        }

        return new(HubMethodFailed);
    }

    public async Task<Response<T>> AllowAnonymous<T>(Request request, Func<Task<Response<T>>> func)
        where T : class?
    {
        try
        {
            return await func.Invoke();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "SessionId: {SessionId}", request.SessionId);
        }

        return new(HubMethodFailed);
    }

    public async Task<Response> RequireSession(Request request, Func<Session, Task<Response>> func)
    {
        try
        {
            return await sessionWrappers.RequireSession(request, func.Invoke);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "SessionId: {SessionId}", request.SessionId);
        }

        return new(HubMethodFailed);
    }

    public async Task<Response<T>> RequireSession<T>(Request request, Func<Session, Task<Response<T>>> func)
        where T : class?
    {
        try
        {
            return await sessionWrappers.RequireSession(request, func.Invoke);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "SessionId: {SessionId}", request.SessionId);
        }

        return new(HubMethodFailed);
    }

    public async Task<Response> RequirePermission(Request request, Guid permissionId, Func<Session, Task<Response>> func, [CallerMemberName] String callingMethod = "", [CallerFilePath] String callingPath = "", [CallerLineNumber] Int32 callingLine = 0)
    {
        try
        {
            var callingMethodName = $"{callingMethod} ({Path.GetFileName(callingPath)}:{callingLine})";

            return await sessionWrappers.RequirePermission(request, permissionId, func.Invoke, callingMethodName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "SessionId: {SessionId}", request.SessionId);
        }

        return new(HubMethodFailed);
    }

    public async Task<Response<T>> RequirePermission<T>(Request request, Guid permissionId, Func<Session, Task<Response<T>>> func, [CallerMemberName] String callingMethod = "", [CallerFilePath] String callingPath = "", [CallerLineNumber] Int32 callingLine = 0)
        where T : class?
    {
        try
        {
            var callingMethodName = $"{callingMethod} ({Path.GetFileName(callingPath)}:{callingLine})";

            return await sessionWrappers.RequirePermission(request, permissionId, func.Invoke, callingMethodName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "SessionId: {SessionId}", request.SessionId);
        }

        return new(HubMethodFailed);
    }

    public async Task<Response> RequireUser(Request request, Func<Session, Task<Response>> func)
    {
        try
        {
            return await sessionWrappers.RequireUser(request, func.Invoke);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "SessionId: {SessionId}", request.SessionId);
        }

        return new(HubMethodFailed);
    }

    public async Task<Response<T>> RequireUser<T>(Request request, Func<Session, Task<Response<T>>> func)
        where T : class?
    {
        try
        {
            return await sessionWrappers.RequireUser(request, func.Invoke);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "SessionId: {SessionId}", request.SessionId);
        }

        return new(HubMethodFailed);
    }
}