using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;

namespace Crudspa.Framework.Core.Server.Contracts.Behavior;

public interface IControllerWrappers
{
    Task<ActionResult> RequireSession(HttpRequest request, Func<Session, Task<ActionResult>> func);
    Task<ActionResult> RequirePermission(HttpRequest request, Guid permissionId, Func<Session, Task<ActionResult>> func, [CallerMemberName] String callingMethod = "");
}