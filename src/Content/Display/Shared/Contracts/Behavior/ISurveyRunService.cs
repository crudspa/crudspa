namespace Crudspa.Content.Display.Shared.Contracts.Behavior;

public interface ISurveyRunService
{
    Task<Response<Survey?>> Fetch(Request<Survey> request);
    Task<Response> Complete(Request<SurveyReply> request);
}