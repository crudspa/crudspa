using Crudspa.Framework.Core.Server.Contracts;

namespace Crudspa.Content.Display.Server.Hubs;

public partial class DisplayHub
{
    public async Task<Response<BinderTypeFull?>> BinderRunFetchBinderType(Request<Binder> request)
    {
        return await HubWrappers.RequireSession(request, async session =>
            await BinderRunService.FetchBinderType(request));
    }

    public async Task<Response<Binder?>> BinderRunFetchBinder(Request<Binder> request)
    {
        return await HubWrappers.RequireSession(request, async session =>
            await BinderRunService.FetchBinder(request));
    }

    public async Task<Response<Page?>> BinderRunFetchPage(Request<Page> request)
    {
        return await HubWrappers.RequireSession(request, async session =>
        {
            var cacheKey = String.Format(CacheKeys.Page, request.Value.Id);

            return await CacheService.GetOrAdd(cacheKey, async () => await BinderRunService.FetchPage(request));
        });
    }

    public async Task<Response> BinderRunAddCompleted(Request<BinderCompleted> request)
    {
        return await HubWrappers.RequireSession(request, async session =>
        {
            if (session.User?.Contact.Id is null)
                return new();

            return await BinderRunService.AddCompleted(request);
        });
    }
}