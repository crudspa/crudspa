namespace Crudspa.Education.Publisher.Shared.Contracts.Behavior;

public interface IAssessmentService
{
    Task<Response<IList<Assessment>>> Search(Request<AssessmentSearch> request);
    Task<Response<Assessment?>> Fetch(Request<Assessment> request);
    Task<Response<Assessment?>> Add(Request<Assessment> request);
    Task<Response> Save(Request<Assessment> request);
    Task<Response> Remove(Request<Assessment> request);
    Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request);
    Task<Response<IList<Orderable>>> FetchGradeNames(Request request);
    Task<Response<IList<Orderable>>> FetchContentCategoryNames(Request request);
}