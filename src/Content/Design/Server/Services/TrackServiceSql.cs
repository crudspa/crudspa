namespace Crudspa.Content.Design.Server.Services;

public class TrackServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IHtmlSanitizer htmlSanitizer)
    : ITrackService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Track>>> FetchForPortal(Request<Portal> request)
    {
        return await wrappers.Try<IList<Track>>(request, async response =>
        {
            var tracks = await TrackSelectForPortal.Execute(Connection, request.SessionId, request.Value.Id);
            return tracks;
        });
    }

    public async Task<Response<Track?>> Fetch(Request<Track> request)
    {
        return await wrappers.Try<Track?>(request, async response =>
        {
            var track = await TrackSelect.Execute(Connection, request.SessionId, request.Value);
            return track;
        });
    }

    public async Task<Response<Track?>> Add(Request<Track> request)
    {
        return await wrappers.Validate<Track?, Track>(request, async response =>
        {
            var track = request.Value;

            track.Description = htmlSanitizer.Sanitize(track.Description);

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await TrackInsert.Execute(connection, transaction, request.SessionId, track);

                return new Track
                {
                    Id = id,
                    PortalId = track.PortalId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<Track> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var track = request.Value;

            track.Description = htmlSanitizer.Sanitize(track.Description);

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await TrackUpdate.Execute(connection, transaction, request.SessionId, track);
            });
        });
    }

    public async Task<Response> Remove(Request<Track> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var track = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await TrackDelete.Execute(connection, transaction, request.SessionId, track);
            });
        });
    }

    public async Task<Response> SaveOrder(Request<IList<Track>> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var tracks = request.Value;

            tracks.EnsureOrder();

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await TrackUpdateOrdinals.Execute(connection, transaction, request.SessionId, tracks);
            });
        });
    }

    public async Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await ContentStatusSelectOrderables.Execute(Connection, request.SessionId));
    }

    public async Task<Response<IList<Named>>> FetchAchievementNames(Request<Portal> request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await AchievementSelectNames.Execute(Connection, request.Value.Id));
    }
}