namespace Crudspa.Samples.Composer.Client.Services;

using Composer = Shared.Contracts.Data.Composer;

public class ComposerServiceTcp(IProxyWrappers proxyWrappers) : IComposerService
{
    public async Task<Response<Composer?>> Fetch(Request request) =>
        await proxyWrappers.Send<Composer?>("ComposerFetch", request);

    public async Task<Response> Save(Request<Composer> request) =>
        await proxyWrappers.Send("ComposerSave", request);

    public async Task<Response<IList<Named>>> FetchPermissionNames(Request request) =>
        await proxyWrappers.Send<IList<Named>>("ComposerFetchPermissionNames", request);
}