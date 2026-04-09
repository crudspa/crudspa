using PermissionIds = Crudspa.Education.Common.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.Publisher.Server.Hubs;

public partial class PublisherHub
{
    public async Task<Response<IList<ListenPart>>> ListenPartFetchForAssessment(Request<Assessment> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
            await ListenPartService.FetchForAssessment(request));
    }

    public async Task<Response<ListenPart?>> ListenPartFetch(Request<ListenPart> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
            await ListenPartService.Fetch(request));
    }

    public async Task<Response<ListenPart?>> ListenPartAdd(Request<ListenPart> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
        {
            var response = await ListenPartService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Assessments, new ListenPartAdded
                {
                    Id = response.Value.Id,
                    AssessmentId = request.Value.AssessmentId,
                });

            return response;
        });
    }

    public async Task<Response> ListenPartSave(Request<ListenPart> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
        {
            var response = await ListenPartService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Assessments, new ListenPartSaved
                {
                    Id = request.Value.Id,
                    AssessmentId = request.Value.AssessmentId,
                });

            return response;
        });
    }

    public async Task<Response> ListenPartRemove(Request<ListenPart> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
        {
            var response = await ListenPartService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Assessments, new ListenPartRemoved
                {
                    Id = request.Value.Id,
                    AssessmentId = request.Value.AssessmentId,
                });

            return response;
        });
    }

    public async Task<Response> ListenPartSaveOrder(Request<IList<ListenPart>> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
        {
            var response = await ListenPartService.SaveOrder(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Assessments, new ListenPartsReordered
                {
                    AssessmentId = request.Value.First().AssessmentId,
                });

            return response;
        });
    }
}