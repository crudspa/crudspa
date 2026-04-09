using System.Runtime.CompilerServices;

namespace Crudspa.Framework.Core.Server.Services;

public class AccessDeniedServiceSql(IServerConfigService configService) : IAccessDeniedService
{
    private String Connection => configService.Fetch().Database;

    public async Task Add(AccessDenied accessDenied, [CallerMemberName] String callingMethod = "")
    {
        if (accessDenied.Method.HasNothing())
            accessDenied.Method = callingMethod;

        if (accessDenied.EventType.HasNothing())
            accessDenied.EventType = Constants.AccessDeniedEventTypes.Unknown;

        await AccessDeniedInsert.Execute(Connection, accessDenied);
    }
}