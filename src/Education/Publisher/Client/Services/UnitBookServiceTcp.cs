namespace Crudspa.Education.Publisher.Client.Services;

public class UnitBookServiceTcp(IProxyWrappers proxyWrappers) : IUnitBookService
{
    public async Task<Response<IList<UnitBook>>> FetchForUnit(Request<Unit> request) =>
        await proxyWrappers.Send<IList<UnitBook>>("UnitBookFetchForUnit", request);

    public async Task<Response<UnitBook?>> Fetch(Request<UnitBook> request) =>
        await proxyWrappers.Send<UnitBook?>("UnitBookFetch", request);

    public async Task<Response<UnitBook?>> Add(Request<UnitBook> request) =>
        await proxyWrappers.Send<UnitBook?>("UnitBookAdd", request);

    public async Task<Response> Save(Request<UnitBook> request) =>
        await proxyWrappers.Send("UnitBookSave", request);

    public async Task<Response> Remove(Request<UnitBook> request) =>
        await proxyWrappers.Send("UnitBookRemove", request);

    public async Task<Response<Copy>> Copy(Request<Copy> request) =>
        await proxyWrappers.Send<Copy>("UnitBookCopy", request);

    public async Task<Response> SaveOrder(Request<IList<UnitBook>> request) =>
        await proxyWrappers.Send("UnitBookSaveOrder", request);

    public async Task<Response<IList<Named>>> FetchBookNames(Request request) =>
        await proxyWrappers.Send<IList<Named>>("UnitBookFetchBookNames", request);
}