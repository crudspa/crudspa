using PermissionIds = Crudspa.Education.Common.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.Publisher.Server.Hubs;

public partial class PublisherHub
{
    public async Task<Response<IList<Assessment>>> AssessmentSearch(Request<AssessmentSearch> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
        {
            request.Value.TimeZoneId = session.User?.Contact.TimeZoneId ?? Constants.DefaultTimeZone;
            return await AssessmentService.Search(request);
        });
    }

    public async Task<Response<Assessment?>> AssessmentFetch(Request<Assessment> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
            await AssessmentService.Fetch(request));
    }

    public async Task<Response<Assessment?>> AssessmentAdd(Request<Assessment> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
        {
            var response = await AssessmentService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Assessments, new AssessmentAdded
                {
                    Id = response.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> AssessmentSave(Request<Assessment> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
        {
            var response = await AssessmentService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Assessments, new AssessmentSaved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response> AssessmentRemove(Request<Assessment> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
        {
            var response = await AssessmentService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Assessments, new AssessmentRemoved
                {
                    Id = request.Value.Id,
                });

            return response;
        });
    }

    public async Task<Response<IList<Orderable>>> AssessmentFetchContentStatusNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
            await AssessmentService.FetchContentStatusNames(request));
    }

    public async Task<Response<IList<Orderable>>> AssessmentFetchGradeNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
            await AssessmentService.FetchGradeNames(request));
    }

    public async Task<Response<IList<Orderable>>> AssessmentFetchContentCategoryNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
            await AssessmentService.FetchContentCategoryNames(request));
    }
}