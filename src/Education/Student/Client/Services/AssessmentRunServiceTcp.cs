namespace Crudspa.Education.Student.Client.Services;

public class AssessmentRunServiceTcp(IProxyWrappers proxyWrappers) : IAssessmentRunService
{
    public async Task<Response<IList<AssessmentAssignment>>> FetchAssessments(Request request) =>
        await proxyWrappers.Send<IList<AssessmentAssignment>>("AssessmentRunFetchAssessments", request);

    public async Task<Response<Assessment?>> FetchAssessment(Request<AssessmentAssignment> request) =>
        await proxyWrappers.Send<Assessment?>("AssessmentRunFetchAssessment", request);

    public async Task<Response> AddVocabChoiceSelection(Request<VocabChoiceSelection> request) =>
        await proxyWrappers.Send("AssessmentRunAddVocabChoiceSelection", request);

    public async Task<Response> AddListenChoiceSelection(Request<ListenChoiceSelection> request) =>
        await proxyWrappers.Send("AssessmentRunAddListenChoiceSelection", request);

    public async Task<Response> AddReadChoiceSelection(Request<ReadChoiceSelection> request) =>
        await proxyWrappers.Send("AssessmentRunAddReadChoiceSelection", request);

    public async Task<Response> AddVocabPartCompleted(Request<VocabPartCompleted> request) =>
        await proxyWrappers.Send("AssessmentRunAddVocabPartCompleted", request);

    public async Task<Response> AddListenPartCompleted(Request<ListenPartCompleted> request) =>
        await proxyWrappers.Send("AssessmentRunAddListenPartCompleted", request);

    public async Task<Response> AddReadPartCompleted(Request<ReadPartCompleted> request) =>
        await proxyWrappers.Send("AssessmentRunAddReadPartCompleted", request);

    public async Task<Response> AddAssessmentCompleted(Request<AssessmentAssignment> request) =>
        await proxyWrappers.Send("AssessmentRunAddAssessmentCompleted", request);
}