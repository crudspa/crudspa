namespace Crudspa.Framework.Core.Server.Services;

public class SessionStateServiceSql(ISessionService sessionService, ISegmentFetcher segmentFetcher)
    : ISessionStateService
{
    public async Task<Response<Session?>> Fetch(Request<Session> request)
    {
        var response = await sessionService.FetchOrCreate(request);

        if (!response.Ok || response.Value is null)
            return response;

        var session = response.Value;
        var segments = await segmentFetcher.Fetch(session.Id, session.PortalId);

        session.Segments = segments.ToObservable();

        return response;
    }
}