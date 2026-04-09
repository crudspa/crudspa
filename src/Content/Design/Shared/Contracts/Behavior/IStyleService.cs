namespace Crudspa.Content.Design.Shared.Contracts.Behavior;

public interface IStyleService
{
    Task<Response<IList<Style>>> FetchForContentPortal(Request<ContentPortal> request);
    Task<Response<Style?>> Fetch(Request<Style> request);
    Task<Response> Save(Request<Style> request);
    Task<Response<IList<RuleFull>>> FetchRules(Request request);
}