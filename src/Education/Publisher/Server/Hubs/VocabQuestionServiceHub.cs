using PermissionIds = Crudspa.Education.Common.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.Publisher.Server.Hubs;

public partial class PublisherHub
{
    public async Task<Response<IList<VocabQuestion>>> VocabQuestionFetchForVocabPart(Request<VocabPart> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
            await VocabQuestionService.FetchForVocabPart(request));
    }

    public async Task<Response<VocabQuestion?>> VocabQuestionFetch(Request<VocabQuestion> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
            await VocabQuestionService.Fetch(request));
    }

    public async Task<Response<VocabQuestion?>> VocabQuestionAdd(Request<VocabQuestion> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
        {
            var response = await VocabQuestionService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Assessments, new VocabQuestionAdded
                {
                    Id = response.Value.Id,
                    VocabPartId = request.Value.VocabPartId,
                });

            return response;
        });
    }

    public async Task<Response> VocabQuestionSave(Request<VocabQuestion> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
        {
            var response = await VocabQuestionService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Assessments, new VocabQuestionSaved
                {
                    Id = request.Value.Id,
                    VocabPartId = request.Value.VocabPartId,
                });

            return response;
        });
    }

    public async Task<Response> VocabQuestionRemove(Request<VocabQuestion> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
        {
            var response = await VocabQuestionService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Assessments, new VocabQuestionRemoved
                {
                    Id = request.Value.Id,
                    VocabPartId = request.Value.VocabPartId,
                });

            return response;
        });
    }

    public async Task<Response> VocabQuestionSaveOrder(Request<IList<VocabQuestion>> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
        {
            var response = await VocabQuestionService.SaveOrder(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Assessments, new VocabQuestionsReordered
                {
                    VocabPartId = request.Value.First().VocabPartId,
                });

            return response;
        });
    }
}