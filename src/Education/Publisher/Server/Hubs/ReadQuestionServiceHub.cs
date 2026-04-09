using PermissionIds = Crudspa.Education.Common.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.Publisher.Server.Hubs;

public partial class PublisherHub
{
    public async Task<Response<IList<ReadQuestion>>> ReadQuestionFetchForReadPart(Request<ReadPart> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
            await ReadQuestionService.FetchForReadPart(request));
    }

    public async Task<Response<ReadQuestion?>> ReadQuestionFetch(Request<ReadQuestion> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
            await ReadQuestionService.Fetch(request));
    }

    public async Task<Response<ReadQuestion?>> ReadQuestionAdd(Request<ReadQuestion> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
        {
            var response = await ReadQuestionService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Assessments, new ReadQuestionAdded
                {
                    Id = response.Value.Id,
                    ReadPartId = request.Value.ReadPartId,
                });

            return response;
        });
    }

    public async Task<Response> ReadQuestionSave(Request<ReadQuestion> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
        {
            var response = await ReadQuestionService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Assessments, new ReadQuestionSaved
                {
                    Id = request.Value.Id,
                    ReadPartId = request.Value.ReadPartId,
                });

            return response;
        });
    }

    public async Task<Response> ReadQuestionRemove(Request<ReadQuestion> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
        {
            var response = await ReadQuestionService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Assessments, new ReadQuestionRemoved
                {
                    Id = request.Value.Id,
                    ReadPartId = request.Value.ReadPartId,
                });

            return response;
        });
    }

    public async Task<Response> ReadQuestionSaveOrder(Request<IList<ReadQuestion>> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
        {
            var response = await ReadQuestionService.SaveOrder(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Assessments, new ReadQuestionsReordered
                {
                    ReadPartId = request.Value.First().ReadPartId,
                });

            return response;
        });
    }

    public async Task<Response<IList<Orderable>>> ReadQuestionFetchReadQuestionCategoryNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
            await ReadQuestionService.FetchReadQuestionCategoryNames(request));
    }

    public async Task<Response<IList<Orderable>>> ReadQuestionFetchReadQuestionTypeNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
            await ReadQuestionService.FetchReadQuestionTypeNames(request));
    }
}