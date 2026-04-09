namespace Crudspa.Framework.Core.Client.Services;

public class SegmentServiceTcp(IProxyWrappers proxyWrappers) : ISegmentService
{
    public async Task<Response<IList<Segment>>> FetchForPortal(Request<Portal> request) =>
        await proxyWrappers.Send<IList<Segment>>("SegmentFetchForPortal", request);

    public async Task<Response<IList<Segment>>> FetchForParent(Request<Segment> request) =>
        await proxyWrappers.Send<IList<Segment>>("SegmentFetchForParent", request);

    public async Task<Response<Segment?>> Fetch(Request<Segment> request) =>
        await proxyWrappers.Send<Segment?>("SegmentFetch", request);

    public async Task<Response<Segment?>> Add(Request<Segment> request) =>
        await proxyWrappers.Send<Segment?>("SegmentAdd", request);

    public async Task<Response> Save(Request<Segment> request) =>
        await proxyWrappers.Send("SegmentSave", request);

    public async Task<Response> Remove(Request<Segment> request) =>
        await proxyWrappers.Send("SegmentRemove", request);

    public async Task<Response> SaveOrder(Request<IList<Segment>> request) =>
        await proxyWrappers.Send("SegmentSaveOrder", request);

    public async Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("SegmentFetchContentStatusNames", request);

    public async Task<Response<IList<Named>>> FetchPermissionNames(Request<Portal> request) =>
        await proxyWrappers.Send<IList<Named>>("SegmentFetchPermissionNames", request);

    public async Task<Response<IList<IconFull>>> FetchIcons(Request request) =>
        await proxyWrappers.SendAndCache<IList<IconFull>>("SegmentFetchIcons", request);

    public async Task<Response<IList<SegmentTypeFull>>> FetchSegmentTypes(Request<Portal> request) =>
        await proxyWrappers.Send<IList<SegmentTypeFull>>("SegmentFetchSegmentTypes", request);

    public async Task<Response<IList<Named>>> FetchLicenseNames(Request request) =>
        await proxyWrappers.Send<IList<Named>>("SegmentFetchLicenseNames", request);

    public async Task<Response<Segment?>> FetchStructure(Request<Segment> request) =>
        await proxyWrappers.Send<Segment?>("SegmentFetchStructure", request);

    public async Task<Response> SaveStructure(Request<Segment> request) =>
        await proxyWrappers.Send("SegmentSaveStructure", request);

    public async Task<Response<IList<Expandable>>> FetchTree(Request request) =>
        await proxyWrappers.Send<IList<Expandable>>("SegmentFetchTree", request);

    public async Task<Response> Move(Request<Segment> request) =>
        await proxyWrappers.Send("SegmentMove", request);
}