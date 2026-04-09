using System.Text.Json;
using Microsoft.AspNetCore.SignalR;

namespace Crudspa.Framework.Core.Server.Hubs;

public partial class CoreHub : Hub
{
    protected ILogger Logger { get; }
    protected ILoggerFactory LoggerFactory { get; }
    protected IHubWrappers HubWrappers { get; }
    protected IAccessDeniedService AccessDeniedService { get; }
    protected IAccountSettingsService AccountSettingsService { get; }
    protected IAddressService AddressService { get; }
    protected IAuthService AuthService { get; }
    protected IGatewayService GatewayService { get; }
    protected ILinkClickService LinkClickService { get; }
    protected IMediaPlayService MediaPlayService { get; }
    protected IPaneService PaneService { get; }
    protected IPortalRunService PortalRunService { get; }
    protected IPortalService PortalService { get; }
    protected ISegmentService SegmentService { get; }
    protected IServerConfigService ServerConfigService { get; }
    protected ISessionFetcher SessionFetcher { get; }
    protected ISessionStateService SessionStateService { get; }

    public CoreHub(
        ILoggerFactory loggerFactory,
        IHubWrappers hubWrappers,
        // CoreHub (Framework)
        IAccessDeniedService accessDeniedService,
        IAccountSettingsService accountSettingsService,
        IAddressService addressService,
        IAuthService authService,
        IGatewayService gatewayService,
        ILinkClickService linkClickService,
        IMediaPlayService mediaPlayService,
        IPaneService paneService,
        IPortalRunService portalRunService,
        IPortalService portalService,
        ISegmentService segmentService,
        IServerConfigService serverConfigService,
        ISessionFetcher sessionFetcher,
        ISessionStateService sessionStateService)
    {
        LoggerFactory = loggerFactory;
        Logger = loggerFactory.CreateLogger(GetType());
        HubWrappers = hubWrappers;
        AccessDeniedService = accessDeniedService;
        AccountSettingsService = accountSettingsService;
        AddressService = addressService;
        AuthService = authService;
        GatewayService = gatewayService;
        LinkClickService = linkClickService;
        MediaPlayService = mediaPlayService;
        PaneService = paneService;
        PortalRunService = portalRunService;
        PortalService = portalService;
        SegmentService = segmentService;
        ServerConfigService = serverConfigService;
        SessionFetcher = sessionFetcher;
        SessionStateService = sessionStateService;
    }

    public async Task Subscribe(Request request)
    {
        await HubWrappers.RequireSession(request, async session =>
        {
            if (session.User?.OrganizationId is not null)
            {
                foreach (var permissionId in session.Permissions)
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"{session.User.OrganizationId.Value:D}|{permissionId:D}");

                if (session.User?.Contact.Id is not null)
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"{session.User.OrganizationId.Value:D}|{session.User.Contact.Id.Value:D}");
            }

            return new();
        });
    }

    protected async Task Notify<T>(Guid? sessionId, Guid? groupId, T eventObject) where T : class
    {
        if (sessionId is null || groupId is null)
            return;

        var session = await SessionFetcher.Fetch(sessionId);

        var organizationId = session?.User?.OrganizationId;

        if (organizationId is null)
            return;

        await Clients.Group($"{organizationId.Value:D}|{groupId.Value:D}").SendAsync("NoticePosted", new Notice<T>(eventObject));
    }

    public Task<Response> Log(Request<ClientLogEntry> request)
    {
        return HubWrappers.AllowAnonymous(request, () =>
        {
            var entry = request.Value;
            var logger = entry.CategoryName.HasSomething() ? LoggerFactory.CreateLogger(entry.CategoryName!) : Logger;
            var eventId = new EventId(entry.EventId, entry.EventName);
            var exception = entry.Exception.HasSomething() ? new Exception(entry.Exception!) : null;
            var message = entry.Message.HasSomething() ? entry.Message! : String.Empty;
            var data = entry.Data
                .Where(x => x.Key.HasSomething())
                .ToDictionary(x => x.Key, x => Unwrap(x.Value));

            data.TryAdd("LogSource", "Client");

            if (request.SessionId is not null)
                data.TryAdd("SessionId", request.SessionId.Value.ToString("D"));

            logger.Log((LogLevel)entry.LogLevel, eventId, data, exception, (_, _) => message);
            return Task.FromResult(new Response());
        });
    }

    private static Object? Unwrap(Object? value)
    {
        return value switch
        {
            JsonElement json => Unwrap(json),
            _ => value,
        };
    }

    private static Object? Unwrap(JsonElement value)
    {
        return value.ValueKind switch
        {
            JsonValueKind.Null => null,
            JsonValueKind.Undefined => null,
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Number => value.TryGetInt32(out var int32) ? int32
                : value.TryGetInt64(out var int64) ? int64
                : value.TryGetDecimal(out var decimalValue) ? decimalValue
                : value.TryGetDouble(out var doubleValue) ? doubleValue
                : value.GetRawText(),
            JsonValueKind.String => value.GetString(),
            JsonValueKind.Array => value.EnumerateArray().Select(Unwrap).ToList(),
            JsonValueKind.Object => value.EnumerateObject().ToDictionary(x => x.Name, x => Unwrap(x.Value)),
            _ => value.GetRawText(),
        };
    }
}