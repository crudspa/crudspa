namespace Crudspa.Framework.Core.Shared.Contracts.Behavior;

public interface IAddressService
{
    Task<Response<IList<Named>>> FetchUsaStateNames(Request request);
}