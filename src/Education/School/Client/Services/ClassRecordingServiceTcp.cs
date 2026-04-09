namespace Crudspa.Education.School.Client.Services;

public class ClassRecordingServiceTcp(IProxyWrappers proxyWrappers) : IClassRecordingService
{
    public async Task<Response<IList<ClassRecording>>> FetchAll(Request request) =>
        await proxyWrappers.Send<IList<ClassRecording>>("ClassRecordingFetchAll", request);

    public async Task<Response<ClassRecording?>> Fetch(Request<ClassRecording> request) =>
        await proxyWrappers.Send<ClassRecording?>("ClassRecordingFetch", request);

    public async Task<Response<ClassRecording?>> Add(Request<ClassRecording> request) =>
        await proxyWrappers.Send<ClassRecording?>("ClassRecordingAdd", request);

    public async Task<Response> Save(Request<ClassRecording> request) =>
        await proxyWrappers.Send("ClassRecordingSave", request);

    public async Task<Response> Remove(Request<ClassRecording> request) =>
        await proxyWrappers.Send("ClassRecordingRemove", request);

    public async Task<Response<IList<Orderable>>> FetchContentCategoryNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("ClassRecordingFetchContentCategoryNames", request);
}