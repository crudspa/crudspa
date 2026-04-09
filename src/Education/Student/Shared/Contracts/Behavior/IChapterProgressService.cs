namespace Crudspa.Education.Student.Shared.Contracts.Behavior;

public interface IChapterProgressService
{
    Task<Response<IList<ChapterProgress>>> FetchAll(Request request);
    Task<ChapterProgress> Fetch(Request<Chapter> request);
    Task<Response> AddCompleted(Request<ChapterCompleted> request);
}