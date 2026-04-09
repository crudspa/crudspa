namespace Crudspa.Content.Display.Client.Services;

public class BinderRunServiceTcp(IProxyWrappers proxyWrappers) : IBinderRunService
{
    public async Task<Response<BinderTypeFull?>> FetchBinderType(Request<Binder> request) =>
        await proxyWrappers.Send<BinderTypeFull?>("BinderRunFetchBinderType", request);

    public async Task<Response<Binder?>> FetchBinder(Request<Binder> request) =>
        await proxyWrappers.Send<Binder?>("BinderRunFetchBinder", request);

    public async Task<Response<Page?>> FetchPage(Request<Page> request) =>
        await proxyWrappers.Send<Page?>("BinderRunFetchPage", request);

    public async Task<Response> AddCompleted(Request<BinderCompleted> request) =>
        await proxyWrappers.Send("BinderRunAddCompleted", request);
}