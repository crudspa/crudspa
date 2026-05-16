namespace Crudspa.Content.Design.Shared.Contracts.Behavior;

public interface IFontFaceService
{
    Task<Response<IList<FontFace>>> FetchForFont(Request<Font> request);
    Task<Response<FontFace?>> Fetch(Request<FontFace> request);
    Task<Response<FontFace?>> Add(Request<FontFace> request);
    Task<Response> Save(Request<FontFace> request);
    Task<Response> Remove(Request<FontFace> request);
}