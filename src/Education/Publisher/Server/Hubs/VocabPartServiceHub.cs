using PermissionIds = Crudspa.Education.Common.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.Publisher.Server.Hubs;

public partial class PublisherHub
{
    public async Task<Response<IList<VocabPart>>> VocabPartFetchForAssessment(Request<Assessment> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
            await VocabPartService.FetchForAssessment(request));
    }

    public async Task<Response<VocabPart?>> VocabPartFetch(Request<VocabPart> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
            await VocabPartService.Fetch(request));
    }

    public async Task<Response<VocabPart?>> VocabPartAdd(Request<VocabPart> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
        {
            var response = await VocabPartService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Assessments, new VocabPartAdded
                {
                    Id = response.Value.Id,
                    AssessmentId = request.Value.AssessmentId,
                });

            return response;
        });
    }

    public async Task<Response> VocabPartSave(Request<VocabPart> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
        {
            var response = await VocabPartService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Assessments, new VocabPartSaved
                {
                    Id = request.Value.Id,
                    AssessmentId = request.Value.AssessmentId,
                });

            return response;
        });
    }

    public async Task<Response> VocabPartRemove(Request<VocabPart> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
        {
            var response = await VocabPartService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Assessments, new VocabPartRemoved
                {
                    Id = request.Value.Id,
                    AssessmentId = request.Value.AssessmentId,
                });

            return response;
        });
    }

    public async Task<Response> VocabPartSaveOrder(Request<IList<VocabPart>> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
        {
            var response = await VocabPartService.SaveOrder(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Assessments, new VocabPartsReordered
                {
                    AssessmentId = request.Value.First().AssessmentId,
                });

            return response;
        });
    }
}