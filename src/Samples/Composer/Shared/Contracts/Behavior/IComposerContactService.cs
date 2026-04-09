namespace Crudspa.Samples.Composer.Shared.Contracts.Behavior;

public interface IComposerContactService
{
    Task<Response<IList<ComposerContact>>> Search(Request<ComposerContactSearch> request);
    Task<Response<ComposerContact?>> Fetch(Request<ComposerContact> request);
    Task<Response<ComposerContact?>> Add(Request<ComposerContact> request);
    Task<Response> Save(Request<ComposerContact> request);
    Task<Response> Remove(Request<ComposerContact> request);
    Task<Response<IList<Named>>> FetchRoleNames(Request request);
    Task<Response> SendAccessCode(Request<ComposerContact> request);
}