namespace Crudspa.Education.Student.Shared.Contracts.Behavior;

public interface IAssessmentRunService
{
    Task<Response<IList<AssessmentAssignment>>> FetchAssessments(Request request);
    Task<Response<Assessment?>> FetchAssessment(Request<AssessmentAssignment> request);
    Task<Response> AddVocabChoiceSelection(Request<VocabChoiceSelection> request);
    Task<Response> AddListenChoiceSelection(Request<ListenChoiceSelection> request);
    Task<Response> AddReadChoiceSelection(Request<ReadChoiceSelection> request);
    Task<Response> AddVocabPartCompleted(Request<VocabPartCompleted> request);
    Task<Response> AddListenPartCompleted(Request<ListenPartCompleted> request);
    Task<Response> AddReadPartCompleted(Request<ReadPartCompleted> request);
    Task<Response> AddAssessmentCompleted(Request<AssessmentAssignment> request);
}