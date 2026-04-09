namespace Crudspa.Education.Publisher.Shared.Contracts.Behavior;

public interface IReadParagraphService
{
    Task<Response<IList<ReadParagraph>>> FetchForReadPart(Request<ReadPart> request);
    Task<Response<ReadParagraph?>> Fetch(Request<ReadParagraph> request);
    Task<Response<ReadParagraph?>> Add(Request<ReadParagraph> request);
    Task<Response> Save(Request<ReadParagraph> request);
    Task<Response> Remove(Request<ReadParagraph> request);
    Task<Response> SaveOrder(Request<IList<ReadParagraph>> request);
}