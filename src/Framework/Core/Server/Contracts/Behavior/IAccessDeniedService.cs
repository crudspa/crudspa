using System.Runtime.CompilerServices;

namespace Crudspa.Framework.Core.Server.Contracts.Behavior;

public interface IAccessDeniedService
{
    Task Add(AccessDenied accessDenied, [CallerMemberName] String callingMethod = "");
}