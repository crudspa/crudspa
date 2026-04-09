namespace Crudspa.Framework.Core.Server.Contracts.Behavior;

public interface ISessionWrappers
{
    Task<Response> RequireSession(Request request, Func<Session, Task<Response>> func);
    Task<Response<T>> RequireSession<T>(Request request, Func<Session, Task<Response<T>>> func) where T : class?;
    Task<Response> RequireUser(Request request, Func<Session, Task<Response>> func);
    Task<Response<T>> RequireUser<T>(Request request, Func<Session, Task<Response<T>>> func) where T : class?;
    Task<Response> RequirePermission(Request request, Guid permissionId, Func<Session, Task<Response>> func, String callingMethodName = "");
    Task<Response<T>> RequirePermission<T>(Request request, Guid permissionId, Func<Session, Task<Response<T>>> func, String callingMethodName = "") where T : class?;
}