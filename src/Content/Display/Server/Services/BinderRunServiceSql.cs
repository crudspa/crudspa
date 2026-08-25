namespace Crudspa.Content.Display.Server.Services;

public class BinderRunServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IPageRunService pageRunService,
    ISessionLicenseResolver sessionLicenseResolver)
    : IBinderRunService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<BinderPane?>> FetchBinderPane(Request<BinderPane> request)
    {
        return await wrappers.Try<BinderPane?>(request, async response =>
            await BinderPaneSelectForPane.Execute(Connection, request.SessionId, request.Value.PaneId));
    }

    public async Task<Response<BinderTypeFull?>> FetchBinderType(Request<Binder> request)
    {
        return await wrappers.Try<BinderTypeFull?>(request, async response =>
        {
            var licenseIds = await sessionLicenseResolver.Fetch(request.SessionId);
            return await TrackContentIsAccessible.Execute(Connection, licenseIds, binderId: request.Value.Id)
                ? await BinderSelectType.Execute(Connection, request.Value.Id)
                : null;
        });
    }

    public async Task<Response<Binder?>> FetchBinder(Request<Binder> request)
    {
        return await wrappers.Try<Binder?>(request, async response =>
        {
            var binder = request.Value;
            var licenseIds = await sessionLicenseResolver.Fetch(request.SessionId);
            if (!await TrackContentIsAccessible.Execute(Connection, licenseIds, binderId: binder.Id))
                return null;

            var lastPageViewedTask = BinderSelectLastPageViewed.Execute(Connection, binder.Id, request.SessionId);
            var pagesTask = PageSelectForBinder.Execute(Connection, binder.Id);

            await Task.WhenAll(lastPageViewedTask, pagesTask);

            binder.LastPageViewed = await lastPageViewedTask;
            binder.Pages = (await pagesTask).ToObservable();

            var initialPageId = binder.LastPageViewed;

            if (binder.Pages.HasItems())
            {
                if (!initialPageId.HasValue || !binder.Pages.Any(x => x.Id.Equals(initialPageId)))
                    initialPageId = binder.Pages.OrderBy(x => x.Ordinal).First().Id;

                if (initialPageId.HasValue)
                    binder.InitialPage = await PageRunSelectContent.Execute(Connection, new() { Id = initialPageId }, request.SessionId);
            }

            return binder;
        });
    }

    public async Task<Response<Page?>> FetchPage(Request<Page> request)
    {
        return await pageRunService.Fetch(request);
    }

    public async Task<Response<BinderProgress>> FetchProgress(Request<Binder> request)
    {
        return await wrappers.Try<BinderProgress>(request, async response =>
        {
            var licenseIds = await sessionLicenseResolver.Fetch(request.SessionId);
            return await TrackContentIsAccessible.Execute(Connection, licenseIds, binderId: request.Value.Id)
                ? await BinderProgressSelect.Execute(Connection, request.SessionId, request.Value.Id)
                : new();
        });
    }

    public async Task<Response> AddCompleted(Request<BinderCompleted> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var licenseIds = await sessionLicenseResolver.Fetch(request.SessionId);
            if (!await TrackContentIsAccessible.Execute(Connection, licenseIds, binderId: request.Value.BinderId))
            {
                response.AddError("Binder is not accessible.");
                return;
            }

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await BinderCompletedInsert.Execute(connection, transaction, request.SessionId, request.Value);
            });
        });
    }
}