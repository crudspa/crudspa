using System.Runtime.CompilerServices;

namespace Crudspa.Framework.Core.Server.Contracts.Behavior;

public interface IHubWrappers
{
    Task<Response> AllowAnonymous(Request request, Func<Task<Response>> func);
    Task<Response<T>> AllowAnonymous<T>(Request request, Func<Task<Response<T>>> func) where T : class?;
    Task<Response> RequireSession(Request request, Func<Session, Task<Response>> func);
    Task<Response<T>> RequireSession<T>(Request request, Func<Session, Task<Response<T>>> func) where T : class?;
    Task<Response> RequirePermission(Request request, Guid permissionId, Func<Session, Task<Response>> func, [CallerMemberName] String callingMethod = "", [CallerFilePath] String callingPath = "", [CallerLineNumber] Int32 callingLine = 0);
    Task<Response<T>> RequirePermission<T>(Request request, Guid permissionId, Func<Session, Task<Response<T>>> func, [CallerMemberName] String callingMethod = "", [CallerFilePath] String callingPath = "", [CallerLineNumber] Int32 callingLine = 0) where T : class?;
    Task<Response> RequireUser(Request request, Func<Session, Task<Response>> func);
    Task<Response<T>> RequireUser<T>(Request request, Func<Session, Task<Response<T>>> func) where T : class?;
}