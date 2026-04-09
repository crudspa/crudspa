namespace Crudspa.Samples.Composer.Shared.Contracts.Behavior;

using Composer = Data.Composer;

public interface IComposerService
{
    Task<Response<Composer?>> Fetch(Request request);
    Task<Response> Save(Request<Composer> request);
    Task<Response<IList<Named>>> FetchPermissionNames(Request request);
}