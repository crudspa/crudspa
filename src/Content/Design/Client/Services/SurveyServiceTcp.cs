namespace Crudspa.Content.Design.Client.Services;

public class SurveyServiceTcp(IProxyWrappers proxyWrappers) : ISurveyService
{
    public async Task<Response<IList<Named>>> FetchNames(Request<Portal> request) =>
        await proxyWrappers.Send<IList<Named>>("SurveyFetchNames", request);

    public async Task<Response<IList<Survey>>> FetchForPortal(Request<Portal> request) =>
        await proxyWrappers.Send<IList<Survey>>("SurveyFetchForPortal", request);

    public async Task<Response<Survey?>> Fetch(Request<Survey> request) =>
        await proxyWrappers.Send<Survey?>("SurveyFetch", request);

    public async Task<Response<Survey?>> Add(Request<Survey> request) =>
        await proxyWrappers.Send<Survey?>("SurveyAdd", request);

    public async Task<Response> Save(Request<Survey> request) =>
        await proxyWrappers.Send("SurveySave", request);

    public async Task<Response> Remove(Request<Survey> request) =>
        await proxyWrappers.Send("SurveyRemove", request);

    public async Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("SurveyFetchContentStatusNames", request);

    public async Task<Response<IList<SurveyPart>>> FetchParts(Request<Survey> request) =>
        await proxyWrappers.Send<IList<SurveyPart>>("SurveyFetchParts", request);

    public async Task<Response<SurveyPart?>> FetchPart(Request<SurveyPart> request) =>
        await proxyWrappers.Send<SurveyPart?>("SurveyFetchPart", request);

    public async Task<Response<SurveyPart?>> AddPart(Request<SurveyPart> request) =>
        await proxyWrappers.Send<SurveyPart?>("SurveyAddPart", request);

    public async Task<Response> SavePart(Request<SurveyPart> request) =>
        await proxyWrappers.Send("SurveySavePart", request);

    public async Task<Response> RemovePart(Request<SurveyPart> request) =>
        await proxyWrappers.Send("SurveyRemovePart", request);

    public async Task<Response> SavePartOrder(Request<IList<SurveyPart>> request) =>
        await proxyWrappers.Send("SurveySavePartOrder", request);

    public async Task<Response<IList<SurveyQuestion>>> FetchQuestions(Request<SurveyPart> request) =>
        await proxyWrappers.Send<IList<SurveyQuestion>>("SurveyFetchQuestions", request);

    public async Task<Response<SurveyQuestion?>> FetchQuestion(Request<SurveyQuestion> request) =>
        await proxyWrappers.Send<SurveyQuestion?>("SurveyFetchQuestion", request);

    public async Task<Response<SurveyQuestion?>> AddQuestion(Request<SurveyQuestion> request) =>
        await proxyWrappers.Send<SurveyQuestion?>("SurveyAddQuestion", request);

    public async Task<Response> SaveQuestion(Request<SurveyQuestion> request) =>
        await proxyWrappers.Send("SurveySaveQuestion", request);

    public async Task<Response> RemoveQuestion(Request<SurveyQuestion> request) =>
        await proxyWrappers.Send("SurveyRemoveQuestion", request);

    public async Task<Response> SaveQuestionOrder(Request<IList<SurveyQuestion>> request) =>
        await proxyWrappers.Send("SurveySaveQuestionOrder", request);
}