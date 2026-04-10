using Azure.Messaging.EventGrid;
using Crudspa.Framework.Jobs.Shared.Contracts.Events;
using Microsoft.AspNetCore.SignalR;

namespace Crudspa.Framework.Jobs.Server.Services;

public static class JobGatewayRelay
{
    public static async Task<Boolean> TryHandle<THub>(EventGridEvent gridEvent, IHubContext<THub> hubContext)
        where THub : Hub
    {
        switch (gridEvent.Subject)
        {
            case "Crudspa.Framework.Core.Shared.Contracts.Events.JobAdded":
                return await TryRelay<THub, JobAdded>(gridEvent, hubContext);

            case "Crudspa.Framework.Core.Shared.Contracts.Events.JobSaved":
                return await TryRelay<THub, JobSaved>(gridEvent, hubContext);

            case "Crudspa.Framework.Core.Shared.Contracts.Events.JobRemoved":
                return await TryRelay<THub, JobRemoved>(gridEvent, hubContext);

            case "Crudspa.Framework.Jobs.Shared.Contracts.Events.JobStatusChanged":
                return await TryRelay<THub, JobStatusChanged>(gridEvent, hubContext);

            case "Crudspa.Framework.Jobs.Shared.Contracts.Events.JobScheduleAdded":
                return await TryRelay<THub, JobScheduleAdded>(gridEvent, hubContext);

            case "Crudspa.Framework.Jobs.Shared.Contracts.Events.JobScheduleSaved":
                return await TryRelay<THub, JobScheduleSaved>(gridEvent, hubContext);

            case "Crudspa.Framework.Jobs.Shared.Contracts.Events.JobScheduleRemoved":
                return await TryRelay<THub, JobScheduleRemoved>(gridEvent, hubContext);

            default:
                return false;
        }
    }

    private static async Task<Boolean> TryRelay<THub, T>(EventGridEvent gridEvent, IHubContext<THub> hubContext)
        where THub : Hub
        where T : class
    {
        var payload = gridEvent.Data.ToString().FromJson<T>();

        if (payload is null)
            return true;

        await hubContext.Clients.All.SendAsync("NoticePosted", new Notice<T>(payload));
        return true;
    }
}