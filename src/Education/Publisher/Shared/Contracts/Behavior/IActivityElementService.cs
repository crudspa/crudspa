namespace Crudspa.Education.Publisher.Shared.Contracts.Behavior;

public interface IActivityElementService
{
    Task<Response<IList<ActivityTypeFull>>> FetchActivityTypes(Request request);
    Task<Response<IList<Named>>> FetchContentAreaNames(Request request);
}