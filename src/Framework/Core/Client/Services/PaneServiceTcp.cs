namespace Crudspa.Framework.Core.Client.Services;

public class PaneServiceTcp(IProxyWrappers proxyWrappers) : IPaneService
{
    public async Task<Response<Pane?>> Fetch(Request<Pane> request) =>
        await proxyWrappers.Send<Pane?>("PaneFetch", request);

    public async Task<Response> Save(Request<Pane> request) =>
        await proxyWrappers.Send("PaneSave", request);

    public async Task<Response> Remove(Request<Pane> request) =>
        await proxyWrappers.Send("PaneRemove", request);

    public async Task<Response> SaveOrder(Request<IList<Pane>> request) =>
        await proxyWrappers.Send("PaneSaveOrder", request);

    public async Task<Response<IList<PaneTypeFull>>> FetchPaneTypes(Request<Portal> request) =>
        await proxyWrappers.Send<IList<PaneTypeFull>>("PaneFetchPaneTypes", request);

    public async Task<Response> Move(Request<Pane> request) =>
        await proxyWrappers.Send("PaneMove", request);
}