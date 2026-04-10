namespace Crudspa.Education.Publisher.Client.Services;

public class ObjectiveServiceTcp(IProxyWrappers proxyWrappers) : IObjectiveService
{
    public async Task<Response<IList<Objective>>> FetchForLesson(Request<Lesson> request) =>
        await proxyWrappers.Send<IList<Objective>>("ObjectiveFetchForLesson", request);

    public async Task<Response<Objective?>> Fetch(Request<Objective> request) =>
        await proxyWrappers.Send<Objective?>("ObjectiveFetch", request);

    public async Task<Response<Objective?>> Add(Request<Objective> request) =>
        await proxyWrappers.Send<Objective?>("ObjectiveAdd", request);

    public async Task<Response> Save(Request<Objective> request) =>
        await proxyWrappers.Send("ObjectiveSave", request);

    public async Task<Response> Remove(Request<Objective> request) =>
        await proxyWrappers.Send("ObjectiveRemove", request);

    public async Task<Response> SaveOrder(Request<IList<Objective>> request) =>
        await proxyWrappers.Send("ObjectiveSaveOrder", request);

    public async Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("ObjectiveFetchContentStatusNames", request);

    public async Task<Response<IList<Named>>> FetchAchievementNames(Request request) =>
        await proxyWrappers.Send<IList<Named>>("ObjectiveFetchAchievementNames", request);

    public async Task<Response<IList<Orderable>>> FetchBinderTypeNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("ObjectiveFetchBinderTypeNames", request);

    public async Task<Response<Copy>> Copy(Request<Copy> request) =>
        await proxyWrappers.Send<Copy>("ObjectiveCopy", request);

    public async Task<Response<IList<Page>>> FetchPages(Request<ObjectivePage> request) =>
        await proxyWrappers.Send<IList<Page>>("ObjectiveFetchPages", request);

    public async Task<Response<Page?>> FetchPage(Request<ObjectivePage> request) =>
        await proxyWrappers.Send<Page?>("ObjectiveFetchPage", request);

    public async Task<Response<Page?>> AddPage(Request<ObjectivePage> request) =>
        await proxyWrappers.Send<Page?>("ObjectiveAddPage", request);

    public async Task<Response> SavePage(Request<ObjectivePage> request) =>
        await proxyWrappers.Send("ObjectiveSavePage", request);

    public async Task<Response> RemovePage(Request<ObjectivePage> request) =>
        await proxyWrappers.Send("ObjectiveRemovePage", request);

    public async Task<Response> SavePageOrder(Request<ObjectivePage> request) =>
        await proxyWrappers.Send("ObjectiveSavePageOrder", request);

    public async Task<Response<IList<Section>>> FetchSections(Request<ObjectiveSection> request) =>
        await proxyWrappers.Send<IList<Section>>("ObjectiveFetchSections", request);

    public async Task<Response<Section?>> FetchSection(Request<ObjectiveSection> request) =>
        await proxyWrappers.Send<Section?>("ObjectiveFetchSection", request);

    public async Task<Response<Section?>> AddSection(Request<ObjectiveSection> request) =>
        await proxyWrappers.Send<Section?>("ObjectiveAddSection", request);

    public async Task<Response<Section?>> DuplicateSection(Request<ObjectiveSection> request) =>
        await proxyWrappers.Send<Section?>("ObjectiveDuplicateSection", request);

    public async Task<Response> SaveSection(Request<ObjectiveSection> request) =>
        await proxyWrappers.Send("ObjectiveSaveSection", request);

    public async Task<Response> RemoveSection(Request<ObjectiveSection> request) =>
        await proxyWrappers.Send("ObjectiveRemoveSection", request);

    public async Task<Response> SaveSectionOrder(Request<ObjectiveSection> request) =>
        await proxyWrappers.Send("ObjectiveSaveSectionOrder", request);
}