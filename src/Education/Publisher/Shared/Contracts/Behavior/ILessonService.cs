namespace Crudspa.Education.Publisher.Shared.Contracts.Behavior;

public interface ILessonService
{
    Task<Response<IList<Lesson>>> FetchForUnit(Request<Unit> request);
    Task<Response<Lesson?>> Fetch(Request<Lesson> request);
    Task<Response<Lesson?>> Add(Request<Lesson> request);
    Task<Response> Save(Request<Lesson> request);
    Task<Response> Remove(Request<Lesson> request);
    Task<Response<Copy>> Copy(Request<Copy> request);
    Task<Response> SaveOrder(Request<IList<Lesson>> request);
    Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request);
    Task<Response<IList<Named>>> FetchAchievementNames(Request request);
}