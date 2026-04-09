namespace Crudspa.Education.Publisher.Client.Services;

public class UnitServiceTcp(IProxyWrappers proxyWrappers) : IUnitService
{
    public async Task<Response<IList<Unit>>> FetchAll(Request request) =>
        await proxyWrappers.Send<IList<Unit>>("UnitFetchAll", request);

    public async Task<Response<Unit?>> Fetch(Request<Unit> request) =>
        await proxyWrappers.Send<Unit?>("UnitFetch", request);

    public async Task<Response<Unit?>> Add(Request<Unit> request) =>
        await proxyWrappers.Send<Unit?>("UnitAdd", request);

    public async Task<Response> Save(Request<Unit> request) =>
        await proxyWrappers.Send("UnitSave", request);

    public async Task<Response> Remove(Request<Unit> request) =>
        await proxyWrappers.Send("UnitRemove", request);

    public async Task<Response<Copy>> Copy(Request<Copy> request) =>
        await proxyWrappers.Send<Copy>("UnitCopy", request);

    public async Task<Response> SaveOrder(Request<IList<Unit>> request) =>
        await proxyWrappers.Send("UnitSaveOrder", request);

    public async Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("UnitFetchContentStatusNames", request);

    public async Task<Response<IList<Orderable>>> FetchGradeNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("UnitFetchGradeNames", request);

    public async Task<Response<IList<Orderable>>> FetchUnitNames(Request request) =>
        await proxyWrappers.Send<IList<Orderable>>("UnitFetchUnitNames", request);

    public async Task<Response<IList<Named>>> FetchAchievementNames(Request request) =>
        await proxyWrappers.Send<IList<Named>>("UnitFetchAchievementNames", request);
}