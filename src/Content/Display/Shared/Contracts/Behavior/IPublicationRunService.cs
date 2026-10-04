namespace Crudspa.Content.Display.Shared.Contracts.Behavior;

public interface IPublicationRunService
{
    Task<Response<IList<PublicationSummary>>> FetchAll(Request request);
    Task<Response<IList<Orderable>>> FetchPublicationResourceTypeNames(Request request);
}