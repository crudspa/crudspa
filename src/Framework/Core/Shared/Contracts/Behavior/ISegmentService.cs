namespace Crudspa.Framework.Core.Shared.Contracts.Behavior;

public interface ISegmentService
{
    Task<Response<IList<Segment>>> FetchForPortal(Request<Portal> request);
    Task<Response<IList<Segment>>> FetchForParent(Request<Segment> request);
    Task<Response<Segment?>> Fetch(Request<Segment> request);
    Task<Response<Segment?>> Add(Request<Segment> request);
    Task<Response> Save(Request<Segment> request);
    Task<Response> Remove(Request<Segment> request);
    Task<Response> SaveOrder(Request<IList<Segment>> request);
    Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request);
    Task<Response<IList<Named>>> FetchPermissionNames(Request<Portal> request);
    Task<Response<IList<IconFull>>> FetchIcons(Request request);
    Task<Response<IList<SegmentTypeFull>>> FetchSegmentTypes(Request<Portal> request);
    Task<Response<IList<Named>>> FetchLicenseNames(Request request);
    Task<Response<Segment?>> FetchStructure(Request<Segment> request);
    Task<Response> SaveStructure(Request<Segment> request);
    Task<Response<IList<Expandable>>> FetchTree(Request request);
    Task<Response> Move(Request<Segment> request);
}