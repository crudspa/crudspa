namespace Crudspa.Content.Design.Client.Services;

public class FontFaceServiceTcp(IProxyWrappers proxyWrappers) : IFontFaceService
{
    public async Task<Response<IList<FontFace>>> FetchForFont(Request<Font> request) =>
        await proxyWrappers.Send<IList<FontFace>>("FontFaceFetchForFont", request);

    public async Task<Response<FontFace?>> Fetch(Request<FontFace> request) =>
        await proxyWrappers.Send<FontFace?>("FontFaceFetch", request);

    public async Task<Response<FontFace?>> Add(Request<FontFace> request) =>
        await proxyWrappers.Send<FontFace?>("FontFaceAdd", request);

    public async Task<Response> Save(Request<FontFace> request) =>
        await proxyWrappers.Send("FontFaceSave", request);

    public async Task<Response> Remove(Request<FontFace> request) =>
        await proxyWrappers.Send("FontFaceRemove", request);
}