namespace Crudspa.Education.Publisher.Shared.Contracts.Behavior;

public interface IUnitBookService
{
    Task<Response<IList<UnitBook>>> FetchForUnit(Request<Unit> request);
    Task<Response<UnitBook?>> Fetch(Request<UnitBook> request);
    Task<Response<UnitBook?>> Add(Request<UnitBook> request);
    Task<Response> Save(Request<UnitBook> request);
    Task<Response> Remove(Request<UnitBook> request);
    Task<Response<Copy>> Copy(Request<Copy> request);
    Task<Response> SaveOrder(Request<IList<UnitBook>> request);
    Task<Response<IList<Named>>> FetchBookNames(Request request);
}