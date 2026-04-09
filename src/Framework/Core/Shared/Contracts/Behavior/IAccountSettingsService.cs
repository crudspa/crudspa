namespace Crudspa.Framework.Core.Shared.Contracts.Behavior;

public interface IAccountSettingsService
{
    Task<Response<User?>> Fetch(Request request);
    Task<Response> Save(Request<User> request);
}