namespace Crudspa.Content.Display.Client.Services;

public class PublicationRunServiceTcp(IProxyWrappers proxyWrappers) : IPublicationRunService
{
    public async Task<Response<IList<PublicationSummary>>> FetchAll(Request request) =>
        await proxyWrappers.Send<IList<PublicationSummary>>("PublicationRunFetchAll", request);


    public async Task<Response<IList<Orderable>>> FetchPublicationResourceTypeNames(Request request) =>
        await proxyWrappers.Send<IList<Orderable>>("PublicationRunFetchPublicationResourceTypeNames", request);
}