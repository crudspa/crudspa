namespace Crudspa.Content.Design.Shared.Contracts.Behavior;

public interface IFontService
{
    Task<Response<IList<Font>>> FetchForContentPortal(Request<ContentPortal> request);
    Task<Response<Font?>> Fetch(Request<Font> request);
    Task<Response<Font?>> Add(Request<Font> request);
    Task<Response> Save(Request<Font> request);
    Task<Response> Remove(Request<Font> request);
    Task<Response<IList<IconFull>>> FetchIcons(Request request);
}