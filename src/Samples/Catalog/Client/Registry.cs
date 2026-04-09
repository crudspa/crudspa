using Crudspa.Framework.Jobs.Client.Services;
using Crudspa.Samples.Catalog.Client.Services;

namespace Crudspa.Samples.Catalog.Client;

public class Registry
{
    public static void RegisterServices(IServiceCollection services)
    {
        // Logging
        services.AddSingleton<ILoggerProvider, ClientLoggerProviderCore>();

        // Crudspa.Framework.Core.Client
        services.AddSingleton<IClickService, ClickServiceCore>();
        services.AddSingleton<ICookieService, CookieServiceCore>();
        services.AddSingleton<IEventBus, EventBusCore>();
        services.AddSingleton<IJsBridge, JsBridgeCore>();
        services.AddSingleton<INavigator, NavigatorCore>();
        services.AddSingleton<IProxyWrappers, ProxyWrappersCore>();
        services.AddSingleton<IScrollService, ScrollServiceCore>();
        services.AddSingleton<ISessionState, SessionStateCore>();
        services.AddSingleton<IUriProvider, UriProviderCore>();

        // Crudspa.Framework.Core.Shared
        services.AddSingleton<IAccountSettingsService, AccountSettingsServiceTcp>();
        services.AddSingleton<IAddressService, AddressServiceTcp>();
        services.AddSingleton<IAuthService, AuthServiceTcpCatalog>();
        services.AddSingleton<ICacheService, CacheServiceDictionary>();
        services.AddSingleton<ILinkClickService, LinkClickServiceTcp>();
        services.AddSingleton<IMediaPlayService, MediaPlayServiceTcp>();
        services.AddSingleton<IPaneService, PaneServiceTcp>();
        services.AddSingleton<IPortalRunService, PortalRunServiceTcp>();
        services.AddSingleton<IPortalService, PortalServiceTcp>();
        services.AddSingleton<ISegmentService, SegmentServiceTcp>();
        services.AddSingleton<ISessionStateService, SessionStateServiceTcp>();

        // Crudspa.Framework.Jobs.Client
        services.AddSingleton<IJobCopyService, JobCopyServiceJobs>();

        // Crudspa.Framework.Jobs.Shared
        services.AddSingleton<IJobScheduleService, JobScheduleServiceTcp>();
        services.AddSingleton<IJobService, JobServiceTcp>();

        // Crudspa.Samples.Catalog.Shared
        services.AddSingleton<IBookService, BookServiceTcp>();
        services.AddSingleton<ICatalogContactService, CatalogContactServiceTcp>();
        services.AddSingleton<ICatalogService, CatalogServiceTcp>();
        services.AddSingleton<IMovieCreditService, MovieCreditServiceTcp>();
        services.AddSingleton<IMovieService, MovieServiceTcp>();
        services.AddSingleton<IShirtOptionService, ShirtOptionServiceTcp>();
        services.AddSingleton<IShirtService, ShirtServiceTcp>();

    }
}