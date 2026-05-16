using PermissionIds = Crudspa.Content.Display.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Content.Design.Server.Hubs;

public partial class DesignHub
{
    public async Task<Response<IList<SmsMessageMedia>>> SmsMessageMediaFetchForSmsMessage(Request<SmsMessage> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Memberships, async session =>
            await SmsMessageMediaService.FetchForSmsMessage(request));
    }
}