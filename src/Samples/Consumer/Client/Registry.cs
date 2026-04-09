namespace Crudspa.Samples.Consumer.Client;

public class Registry
{
    public static void RegisterServices(IServiceCollection services)
    {
        // Logging
        services.AddSingleton<ILoggerProvider, ClientLoggerProviderCore>();

        // Crudspa.Content.Display.Shared
        services.AddSingleton<IBinderRunService, BinderRunServiceTcp>();
        services.AddSingleton<IBlogRunService, BlogRunServiceTcp>();
        services.AddSingleton<IContactAchievementService, ContactAchievementServiceTcp>();
        services.AddSingleton<IContentPortalRunService, ContentPortalRunServiceTcp>();
        services.AddSingleton<ICourseRunService, CourseRunServiceTcp>();
        services.AddSingleton<IElementProgressService, ElementProgressServiceTcp>();
        services.AddSingleton<INotebookRunService, NotebookRunServiceTcp>();
        services.AddSingleton<IPageRunService, PageRunServiceTcp>();

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
        services.AddSingleton<IAuthService, AuthServiceTcpEmailTfa>();
        services.AddSingleton<ICacheService, CacheServiceDictionary>();
        services.AddSingleton<ILinkClickService, LinkClickServiceTcp>();
        services.AddSingleton<IMediaPlayService, MediaPlayServiceTcp>();
        services.AddSingleton<IPaneService, PaneServiceTcp>();
        services.AddSingleton<IPortalRunService, PortalRunServiceTcp>();
        services.AddSingleton<IPortalService, PortalServiceTcp>();
        services.AddSingleton<ISegmentService, SegmentServiceTcp>();
        services.AddSingleton<ISessionStateService, SessionStateServiceTcp>();
    }
}