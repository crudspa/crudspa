namespace Crudspa.Content.Design.Shared.Contracts.Behavior;

public interface ISectionService
{
    Task<Response<IList<Section>>> FetchForPage(Request<Page> request);
    Task<Response<Section?>> Fetch(Request<Section> request);
    Task<Response<Section?>> Add(Request<Section> request);
    Task<Response> Save(Request<Section> request);
    Task<Response> Remove(Request<Section> request);
    Task<Response<Copy>> Copy(Request<Copy> request);
    Task<Response> SaveOrder(Request<IList<Section>> request);
    Task<Response<IList<ElementType>>> FetchElementTypes(Request request);
    Task<Response<SectionElement?>> CreateElement(Request<ElementSpec> request);
}