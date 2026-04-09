namespace Crudspa.Education.Publisher.Shared.Contracts.Behavior;

public interface IGameService
{
    Task<Response<IList<Game>>> SearchForBook(Request<GameSearch> request);
    Task<Response<Game?>> Fetch(Request<Game> request);
    Task<Response<Game?>> Add(Request<Game> request);
    Task<Response> Save(Request<Game> request);
    Task<Response> Remove(Request<Game> request);
    Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request);
    Task<Response<IList<IconFull>>> FetchIcons(Request request);
    Task<Response<IList<Orderable>>> FetchGradeNames(Request request);
    Task<Response<IList<Orderable>>> FetchAssessmentLevelNames(Request request);
    Task<Response<IList<Named>>> FetchAchievementNames(Request request);
}