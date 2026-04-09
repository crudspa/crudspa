namespace Crudspa.Education.Publisher.Client.Services;

public class ClassRecordingServiceTcp(IProxyWrappers proxyWrappers) : IClassRecordingService
{
    public async Task<Response<IList<ClassRecording>>> Search(Request<ClassRecordingSearch> request) =>
        await proxyWrappers.Send<IList<ClassRecording>>("ClassRecordingSearch", request);
}