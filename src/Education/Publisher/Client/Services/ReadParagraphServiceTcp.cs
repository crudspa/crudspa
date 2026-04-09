namespace Crudspa.Education.Publisher.Client.Services;

public class ReadParagraphServiceTcp(IProxyWrappers proxyWrappers) : IReadParagraphService
{
    public async Task<Response<IList<ReadParagraph>>> FetchForReadPart(Request<ReadPart> request) =>
        await proxyWrappers.Send<IList<ReadParagraph>>("ReadParagraphFetchForReadPart", request);

    public async Task<Response<ReadParagraph?>> Fetch(Request<ReadParagraph> request) =>
        await proxyWrappers.Send<ReadParagraph?>("ReadParagraphFetch", request);

    public async Task<Response<ReadParagraph?>> Add(Request<ReadParagraph> request) =>
        await proxyWrappers.Send<ReadParagraph?>("ReadParagraphAdd", request);

    public async Task<Response> Save(Request<ReadParagraph> request) =>
        await proxyWrappers.Send("ReadParagraphSave", request);

    public async Task<Response> Remove(Request<ReadParagraph> request) =>
        await proxyWrappers.Send("ReadParagraphRemove", request);

    public async Task<Response> SaveOrder(Request<IList<ReadParagraph>> request) =>
        await proxyWrappers.Send("ReadParagraphSaveOrder", request);
}