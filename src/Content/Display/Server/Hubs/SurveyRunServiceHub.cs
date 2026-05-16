namespace Crudspa.Content.Display.Server.Hubs;

public partial class DisplayHub
{
    public async Task<Response<Survey?>> SurveyRunFetch(Request<Survey> request) =>
        await HubWrappers.AllowAnonymous(request, async () => await SurveyRunService.Fetch(request));

    public async Task<Response> SurveyRunComplete(Request<SurveyReply> request) =>
        await HubWrappers.AllowAnonymous(request, async () => await SurveyRunService.Complete(request));
}