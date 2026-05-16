namespace Crudspa.Content.Display.Client.Services;

public class SurveyRunServiceTcp(IProxyWrappers proxyWrappers) : ISurveyRunService
{
    public async Task<Response<Survey?>> Fetch(Request<Survey> request) =>
        await proxyWrappers.Send<Survey?>("SurveyRunFetch", request);

    public async Task<Response> Complete(Request<SurveyReply> request) =>
        await proxyWrappers.Send("SurveyRunComplete", request);
}