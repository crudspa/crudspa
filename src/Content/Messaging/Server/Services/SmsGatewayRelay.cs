using Azure.Messaging.EventGrid;
using Microsoft.AspNetCore.SignalR;

namespace Crudspa.Content.Messaging.Server.Services;

public static class SmsGatewayRelay
{
    public static async Task<Boolean> TryHandle<THub>(EventGridEvent gridEvent, IHubContext<THub> hubContext)
        where THub : Hub
    {
        switch (gridEvent.Subject)
        {
            case "Crudspa.Content.Messaging.Shared.Contracts.Events.SmsAdded":
                return await TryPublish<SmsAdded, THub>(gridEvent, hubContext);

            case "Crudspa.Content.Messaging.Shared.Contracts.Events.SmsSaved":
                return await TryPublish<SmsSaved, THub>(gridEvent, hubContext);

            case "Crudspa.Content.Messaging.Shared.Contracts.Events.SmsRemoved":
                return await TryPublish<SmsRemoved, THub>(gridEvent, hubContext);

            case "Crudspa.Content.Messaging.Shared.Contracts.Events.SmsMessageAdded":
                return await TryPublish<SmsMessageAdded, THub>(gridEvent, hubContext);

            case "Crudspa.Content.Messaging.Shared.Contracts.Events.SmsMessageSaved":
                return await TryPublish<SmsMessageSaved, THub>(gridEvent, hubContext);

            case "Crudspa.Content.Messaging.Shared.Contracts.Events.SmsMessageRemoved":
                return await TryPublish<SmsMessageRemoved, THub>(gridEvent, hubContext);

            case "Crudspa.Content.Messaging.Shared.Contracts.Events.SmsEventAdded":
                return await TryPublish<SmsEventAdded, THub>(gridEvent, hubContext);

            case "Crudspa.Content.Messaging.Shared.Contracts.Events.SmsEventSaved":
                return await TryPublish<SmsEventSaved, THub>(gridEvent, hubContext);

            case "Crudspa.Content.Messaging.Shared.Contracts.Events.SmsEventRemoved":
                return await TryPublish<SmsEventRemoved, THub>(gridEvent, hubContext);

            default:
                return false;
        }
    }

    private static async Task<Boolean> TryPublish<T, THub>(EventGridEvent gridEvent, IHubContext<THub> hubContext)
        where T : class
        where THub : Hub
    {
        var payload = gridEvent.Data.ToString().FromJson<T>();

        if (payload is not null)
            await hubContext.Clients.All.SendAsync("NoticePosted", new Notice<T>(payload));

        return true;
    }
}