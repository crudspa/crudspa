namespace Crudspa.Education.Publisher.Shared.Contracts.Behavior;

public interface IObjectiveService
{
    Task<Response<IList<Objective>>> FetchForLesson(Request<Lesson> request);
    Task<Response<Objective?>> Fetch(Request<Objective> request);
    Task<Response<Objective?>> Add(Request<Objective> request);
    Task<Response> Save(Request<Objective> request);
    Task<Response> Remove(Request<Objective> request);
    Task<Response> SaveOrder(Request<IList<Objective>> request);
    Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request);
    Task<Response<IList<Named>>> FetchAchievementNames(Request request);
    Task<Response<IList<Orderable>>> FetchBinderTypeNames(Request request);
    Task<Response<Copy>> Copy(Request<Copy> request);
    Task<Response<IList<Page>>> FetchPages(Request<ObjectivePage> request);
    Task<Response<Page?>> FetchPage(Request<ObjectivePage> request);
    Task<Response<Page?>> AddPage(Request<ObjectivePage> request);
    Task<Response> SavePage(Request<ObjectivePage> request);
    Task<Response> RemovePage(Request<ObjectivePage> request);
    Task<Response> SavePageOrder(Request<ObjectivePage> request);
    Task<Response<IList<Section>>> FetchSections(Request<ObjectiveSection> request);
    Task<Response<Section?>> FetchSection(Request<ObjectiveSection> request);
    Task<Response<Section?>> AddSection(Request<ObjectiveSection> request);
    Task<Response> SaveSection(Request<ObjectiveSection> request);
    Task<Response> RemoveSection(Request<ObjectiveSection> request);
    Task<Response> SaveSectionOrder(Request<ObjectiveSection> request);
}