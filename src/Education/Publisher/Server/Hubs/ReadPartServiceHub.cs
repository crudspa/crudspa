using PermissionIds = Crudspa.Education.Common.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.Publisher.Server.Hubs;

public partial class PublisherHub
{
    public async Task<Response<IList<ReadPart>>> ReadPartFetchForAssessment(Request<Assessment> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
            await ReadPartService.FetchForAssessment(request));
    }

    public async Task<Response<ReadPart?>> ReadPartFetch(Request<ReadPart> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
            await ReadPartService.Fetch(request));
    }

    public async Task<Response<ReadPart?>> ReadPartAdd(Request<ReadPart> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
        {
            var response = await ReadPartService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Assessments, new ReadPartAdded
                {
                    Id = response.Value.Id,
                    AssessmentId = request.Value.AssessmentId,
                });

            return response;
        });
    }

    public async Task<Response> ReadPartSave(Request<ReadPart> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
        {
            var response = await ReadPartService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Assessments, new ReadPartSaved
                {
                    Id = request.Value.Id,
                    AssessmentId = request.Value.AssessmentId,
                });

            return response;
        });
    }

    public async Task<Response> ReadPartRemove(Request<ReadPart> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
        {
            var response = await ReadPartService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Assessments, new ReadPartRemoved
                {
                    Id = request.Value.Id,
                    AssessmentId = request.Value.AssessmentId,
                });

            return response;
        });
    }

    public async Task<Response> ReadPartSaveOrder(Request<IList<ReadPart>> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
        {
            var response = await ReadPartService.SaveOrder(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Assessments, new ReadPartsReordered
                {
                    AssessmentId = request.Value.First().AssessmentId,
                });

            return response;
        });
    }
}