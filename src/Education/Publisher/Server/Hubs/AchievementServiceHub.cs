using Achievement = Crudspa.Education.Publisher.Shared.Contracts.Data.Achievement;
using AchievementAdded = Crudspa.Education.Publisher.Shared.Contracts.Events.AchievementAdded;
using AchievementRemoved = Crudspa.Education.Publisher.Shared.Contracts.Events.AchievementRemoved;
using AchievementSaved = Crudspa.Education.Publisher.Shared.Contracts.Events.AchievementSaved;
using PermissionIds = Crudspa.Content.Display.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.Publisher.Server.Hubs;

public partial class PublisherHub
{
    public async Task<Response<IList<Achievement>>> EducationAchievementSearch(Request<AchievementSearch> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Achievements, async session =>
        {
            request.Value.TimeZoneId = session.User?.Contact.TimeZoneId ?? Constants.DefaultTimeZone;
            return await EducationAchievementService.Search(request);
        });
    }

    public async Task<Response<Achievement?>> EducationAchievementFetch(Request<Achievement> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Achievements, async session =>
            await EducationAchievementService.Fetch(request));
    }

    public async Task<Response<Achievement?>> EducationAchievementAdd(Request<Achievement> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Achievements, async session =>
        {
            var response = await EducationAchievementService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Achievements, new AchievementAdded
                {
                    Id = response.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> EducationAchievementSave(Request<Achievement> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Achievements, async session =>
        {
            var response = await EducationAchievementService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Achievements, new AchievementSaved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> EducationAchievementRemove(Request<Achievement> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Achievements, async session =>
        {
            var response = await EducationAchievementService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Achievements, new AchievementRemoved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response<IList<Orderable>>> EducationAchievementFetchRarityNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Achievements, async session =>
            await EducationAchievementService.FetchRarityNames(request));
    }
}