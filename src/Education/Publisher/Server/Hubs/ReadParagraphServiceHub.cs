using PermissionIds = Crudspa.Education.Common.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Education.Publisher.Server.Hubs;

public partial class PublisherHub
{
    public async Task<Response<IList<ReadParagraph>>> ReadParagraphFetchForReadPart(Request<ReadPart> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
            await ReadParagraphService.FetchForReadPart(request));
    }

    public async Task<Response<ReadParagraph?>> ReadParagraphFetch(Request<ReadParagraph> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
            await ReadParagraphService.Fetch(request));
    }

    public async Task<Response<ReadParagraph?>> ReadParagraphAdd(Request<ReadParagraph> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
        {
            var response = await ReadParagraphService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Assessments, new ReadParagraphAdded
                {
                    Id = response.Value.Id,
                    ReadPartId = request.Value.ReadPartId,
                });

            return response;
        });
    }

    public async Task<Response> ReadParagraphSave(Request<ReadParagraph> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
        {
            var response = await ReadParagraphService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Assessments, new ReadParagraphSaved
                {
                    Id = request.Value.Id,
                    ReadPartId = request.Value.ReadPartId,
                });

            return response;
        });
    }

    public async Task<Response> ReadParagraphRemove(Request<ReadParagraph> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
        {
            var response = await ReadParagraphService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Assessments, new ReadParagraphRemoved
                {
                    Id = request.Value.Id,
                    ReadPartId = request.Value.ReadPartId,
                });

            return response;
        });
    }

    public async Task<Response> ReadParagraphSaveOrder(Request<IList<ReadParagraph>> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Assessments, async session =>
        {
            var response = await ReadParagraphService.SaveOrder(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Assessments, new ReadParagraphsReordered
                {
                    ReadPartId = request.Value.First().ReadPartId,
                });

            return response;
        });
    }
}