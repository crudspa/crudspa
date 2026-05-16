using PermissionIds = Crudspa.Content.Display.Shared.Contracts.Ids.PermissionIds;

namespace Crudspa.Content.Design.Server.Hubs;

public partial class DesignHub
{
    public async Task<Response<IList<Named>>> SurveyFetchNames(Request<Portal> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Surveys, async session =>
            await SurveyService.FetchNames(request));
    }

    public async Task<Response<IList<Survey>>> SurveyFetchForPortal(Request<Portal> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Surveys, async session =>
            await SurveyService.FetchForPortal(request));
    }

    public async Task<Response<Survey?>> SurveyFetch(Request<Survey> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Surveys, async session =>
            await SurveyService.Fetch(request));
    }

    public async Task<Response<Survey?>> SurveyAdd(Request<Survey> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Surveys, async session =>
        {
            var response = await SurveyService.Add(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Surveys, new SurveyAdded
                {
                    Id = response.Value?.Id,
                    PortalId = response.Value?.PortalId,
                });

            return response;
        });
    }

    public async Task<Response> SurveySave(Request<Survey> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Surveys, async session =>
        {
            var response = await SurveyService.Save(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Surveys, new SurveySaved
                {
                    Id = request.Value.Id,
                    PortalId = request.Value.PortalId,
                });

            return response;
        });
    }

    public async Task<Response> SurveyRemove(Request<Survey> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Surveys, async session =>
        {
            var response = await SurveyService.Remove(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Surveys, new SurveyRemoved
                {
                    Id = request.Value.Id,
                    PortalId = request.Value.PortalId,
                });

            return response;
        });
    }

    public async Task<Response<IList<Orderable>>> SurveyFetchContentStatusNames(Request request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Surveys, async session =>
            await SurveyService.FetchContentStatusNames(request));
    }

    public async Task<Response<IList<SurveyPart>>> SurveyFetchParts(Request<Survey> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Surveys, async session =>
            await SurveyService.FetchParts(request));
    }

    public async Task<Response<SurveyPart?>> SurveyFetchPart(Request<SurveyPart> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Surveys, async session =>
            await SurveyService.FetchPart(request));
    }

    public async Task<Response<SurveyPart?>> SurveyAddPart(Request<SurveyPart> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Surveys, async session =>
        {
            var response = await SurveyService.AddPart(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Surveys, new SurveyPartAdded
                {
                    Id = response.Value?.Id,
                    SurveyId = response.Value?.SurveyId,
                });

            return response;
        });
    }

    public async Task<Response> SurveySavePart(Request<SurveyPart> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Surveys, async session =>
        {
            var response = await SurveyService.SavePart(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Surveys, new SurveyPartSaved
                {
                    Id = request.Value.Id,
                    SurveyId = request.Value.SurveyId,
                });

            return response;
        });
    }

    public async Task<Response> SurveyRemovePart(Request<SurveyPart> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Surveys, async session =>
        {
            var response = await SurveyService.RemovePart(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Surveys, new SurveyPartRemoved
                {
                    Id = request.Value.Id,
                    SurveyId = request.Value.SurveyId,
                });

            return response;
        });
    }

    public async Task<Response> SurveySavePartOrder(Request<IList<SurveyPart>> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Surveys, async session =>
        {
            var response = await SurveyService.SavePartOrder(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Surveys, new SurveyPartsReordered
                {
                    SurveyId = request.Value.FirstOrDefault()?.SurveyId,
                });

            return response;
        });
    }

    public async Task<Response<IList<SurveyQuestion>>> SurveyFetchQuestions(Request<SurveyPart> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Surveys, async session =>
            await SurveyService.FetchQuestions(request));
    }

    public async Task<Response<SurveyQuestion?>> SurveyFetchQuestion(Request<SurveyQuestion> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Surveys, async session =>
            await SurveyService.FetchQuestion(request));
    }

    public async Task<Response<SurveyQuestion?>> SurveyAddQuestion(Request<SurveyQuestion> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Surveys, async session =>
        {
            var response = await SurveyService.AddQuestion(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Surveys, new SurveyQuestionAdded
                {
                    Id = response.Value?.Id,
                    PartId = response.Value?.PartId,
                });

            return response;
        });
    }

    public async Task<Response> SurveySaveQuestion(Request<SurveyQuestion> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Surveys, async session =>
        {
            var response = await SurveyService.SaveQuestion(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Surveys, new SurveyQuestionSaved
                {
                    Id = request.Value.Id,
                    PartId = request.Value.PartId,
                });

            return response;
        });
    }

    public async Task<Response> SurveyRemoveQuestion(Request<SurveyQuestion> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Surveys, async session =>
        {
            var response = await SurveyService.RemoveQuestion(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Surveys, new SurveyQuestionRemoved
                {
                    Id = request.Value.Id,
                    PartId = request.Value.PartId,
                });

            return response;
        });
    }

    public async Task<Response> SurveySaveQuestionOrder(Request<IList<SurveyQuestion>> request)
    {
        return await HubWrappers.RequirePermission(request, PermissionIds.Surveys, async session =>
        {
            var response = await SurveyService.SaveQuestionOrder(request);

            if (response.Ok)
                await Notify(request.SessionId, PermissionIds.Surveys, new SurveyQuestionsReordered
                {
                    PartId = request.Value.FirstOrDefault()?.PartId,
                });

            return response;
        });
    }
}