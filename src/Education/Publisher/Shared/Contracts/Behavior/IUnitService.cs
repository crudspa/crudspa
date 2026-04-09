namespace Crudspa.Education.Publisher.Shared.Contracts.Behavior;

public interface IUnitService
{
    Task<Response<IList<Unit>>> FetchAll(Request request);
    Task<Response<Unit?>> Fetch(Request<Unit> request);
    Task<Response<Unit?>> Add(Request<Unit> request);
    Task<Response> Save(Request<Unit> request);
    Task<Response> Remove(Request<Unit> request);
    Task<Response<Copy>> Copy(Request<Copy> request);
    Task<Response> SaveOrder(Request<IList<Unit>> request);
    Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request);
    Task<Response<IList<Orderable>>> FetchGradeNames(Request request);
    Task<Response<IList<Orderable>>> FetchUnitNames(Request request);
    Task<Response<IList<Named>>> FetchAchievementNames(Request request);
}