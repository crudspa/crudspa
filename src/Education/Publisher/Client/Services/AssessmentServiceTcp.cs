namespace Crudspa.Education.Publisher.Client.Services;

public class AssessmentServiceTcp(IProxyWrappers proxyWrappers) : IAssessmentService
{
    public async Task<Response<IList<Assessment>>> Search(Request<AssessmentSearch> request) =>
        await proxyWrappers.Send<IList<Assessment>>("AssessmentSearch", request);

    public async Task<Response<Assessment?>> Fetch(Request<Assessment> request) =>
        await proxyWrappers.Send<Assessment?>("AssessmentFetch", request);

    public async Task<Response<Assessment?>> Add(Request<Assessment> request) =>
        await proxyWrappers.Send<Assessment?>("AssessmentAdd", request);

    public async Task<Response> Save(Request<Assessment> request) =>
        await proxyWrappers.Send("AssessmentSave", request);

    public async Task<Response> Remove(Request<Assessment> request) =>
        await proxyWrappers.Send("AssessmentRemove", request);

    public async Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("AssessmentFetchContentStatusNames", request);

    public async Task<Response<IList<Orderable>>> FetchGradeNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("AssessmentFetchGradeNames", request);

    public async Task<Response<IList<Orderable>>> FetchContentCategoryNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("AssessmentFetchContentCategoryNames", request);
}