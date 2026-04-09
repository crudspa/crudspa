using PermissionIds = Crudspa.Education.Common.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.Publisher.Server.Hubs;

public partial class PublisherHub
{
    public async Task<Response<IList<ListenQuestion>>> ListenQuestionFetchForListenPart(Request<ListenPart> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
            await ListenQuestionService.FetchForListenPart(request));
    }

    public async Task<Response<ListenQuestion?>> ListenQuestionFetch(Request<ListenQuestion> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
            await ListenQuestionService.Fetch(request));
    }

    public async Task<Response<ListenQuestion?>> ListenQuestionAdd(Request<ListenQuestion> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
        {
            var response = await ListenQuestionService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Assessments, new ListenQuestionAdded
                {
                    Id = response.Value.Id,
                    ListenPartId = request.Value.ListenPartId,
                });

            return response;
        });
    }

    public async Task<Response> ListenQuestionSave(Request<ListenQuestion> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
        {
            var response = await ListenQuestionService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Assessments, new ListenQuestionSaved
                {
                    Id = request.Value.Id,
                    ListenPartId = request.Value.ListenPartId,
                });

            return response;
        });
    }

    public async Task<Response> ListenQuestionRemove(Request<ListenQuestion> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
        {
            var response = await ListenQuestionService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Assessments, new ListenQuestionRemoved
                {
                    Id = request.Value.Id,
                    ListenPartId = request.Value.ListenPartId,
                });

            return response;
        });
    }

    public async Task<Response> ListenQuestionSaveOrder(Request<IList<ListenQuestion>> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
        {
            var response = await ListenQuestionService.SaveOrder(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Assessments, new ListenQuestionsReordered
                {
                    ListenPartId = request.Value.First().ListenPartId,
                });

            return response;
        });
    }

    public async Task<Response<IList<Orderable>>> ListenQuestionFetchListenQuestionCategoryNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
            await ListenQuestionService.FetchListenQuestionCategoryNames(request));
    }

    public async Task<Response<IList<Orderable>>> ListenQuestionFetchReadQuestionTypeNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
            await ListenQuestionService.FetchReadQuestionTypeNames(request));
    }
}