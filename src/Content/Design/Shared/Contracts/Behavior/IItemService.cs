namespace Crudspa.Content.Design.Shared.Contracts.Behavior;

public interface IItemService
{
    Task<Response<IList<Orderable>>> FetchBasisNames(Request request);
    Task<Response<IList<Orderable>>> FetchAlignSelfNames(Request request);
}