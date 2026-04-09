namespace Crudspa.Education.Common.Shared.Contracts.Behavior;

public interface IReportService
{
    Task<Response<IList<Report>>> FetchAll(Request request);
    Task<Response<Report?>> Fetch(Request<Report> request);
    Task<Response<IList<Named>>> FetchReportNames(Request<Portal> request);
}