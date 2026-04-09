namespace Crudspa.Samples.Composer.Client.Services;

public class ComposerContactServiceTcp(IProxyWrappers proxyWrappers) : IComposerContactService
{
    public async Task<Response<IList<ComposerContact>>> Search(Request<ComposerContactSearch> request) =>
        await proxyWrappers.Send<IList<ComposerContact>>("ComposerContactSearch", request);

    public async Task<Response<ComposerContact?>> Fetch(Request<ComposerContact> request) =>
        await proxyWrappers.Send<ComposerContact?>("ComposerContactFetch", request);

    public async Task<Response<ComposerContact?>> Add(Request<ComposerContact> request) =>
        await proxyWrappers.Send<ComposerContact?>("ComposerContactAdd", request);

    public async Task<Response> Save(Request<ComposerContact> request) =>
        await proxyWrappers.Send("ComposerContactSave", request);

    public async Task<Response> Remove(Request<ComposerContact> request) =>
        await proxyWrappers.Send("ComposerContactRemove", request);

    public async Task<Response<IList<Named>>> FetchRoleNames(Request request) =>
        await proxyWrappers.Send<IList<Named>>("ComposerContactFetchRoleNames", request);

    public async Task<Response> SendAccessCode(Request<ComposerContact> request) =>
        await proxyWrappers.Send("ComposerContactSendAccessCode", request);
}