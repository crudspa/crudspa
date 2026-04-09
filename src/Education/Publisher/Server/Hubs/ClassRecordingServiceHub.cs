using PermissionIds = Crudspa.Education.Common.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.Publisher.Server.Hubs;

public partial class PublisherHub
{
    public async Task<Response<IList<ClassRecording>>> ClassRecordingSearch(Request<ClassRecordingSearch> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.ClassRecording, async session =>
        {
            request.Value.TimeZoneId = session.User?.Contact.TimeZoneId ?? Constants.DefaultTimeZone;
            return await ClassRecordingService.Search(request);
        });
    }
}