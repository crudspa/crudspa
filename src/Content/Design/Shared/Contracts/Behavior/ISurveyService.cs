namespace Crudspa.Content.Design.Shared.Contracts.Behavior;

public interface ISurveyService
{
    Task<Response<IList<Named>>> FetchNames(Request<Portal> request);
    Task<Response<IList<Survey>>> FetchForPortal(Request<Portal> request);
    Task<Response<Survey?>> Fetch(Request<Survey> request);
    Task<Response<Survey?>> Add(Request<Survey> request);
    Task<Response> Save(Request<Survey> request);
    Task<Response> Remove(Request<Survey> request);
    Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request);
    Task<Response<IList<SurveyPart>>> FetchParts(Request<Survey> request);
    Task<Response<SurveyPart?>> FetchPart(Request<SurveyPart> request);
    Task<Response<SurveyPart?>> AddPart(Request<SurveyPart> request);
    Task<Response> SavePart(Request<SurveyPart> request);
    Task<Response> RemovePart(Request<SurveyPart> request);
    Task<Response> SavePartOrder(Request<IList<SurveyPart>> request);
    Task<Response<IList<SurveyQuestion>>> FetchQuestions(Request<SurveyPart> request);
    Task<Response<SurveyQuestion?>> FetchQuestion(Request<SurveyQuestion> request);
    Task<Response<SurveyQuestion?>> AddQuestion(Request<SurveyQuestion> request);
    Task<Response> SaveQuestion(Request<SurveyQuestion> request);
    Task<Response> RemoveQuestion(Request<SurveyQuestion> request);
    Task<Response> SaveQuestionOrder(Request<IList<SurveyQuestion>> request);
}