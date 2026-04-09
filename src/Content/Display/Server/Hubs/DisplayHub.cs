namespace Crudspa.Content.Display.Server.Hubs;

public partial class DisplayHub : Crudspa.Framework.Core.Server.Hubs.CoreHub
{
    protected IBinderRunService BinderRunService { get; }
    protected IBlogRunService BlogRunService { get; }
    protected ICacheService CacheService { get; }
    protected IContactAchievementService ContactAchievementService { get; }
    protected IContentPortalRunService ContentPortalRunService { get; }
    protected ICourseRunService CourseRunService { get; }
    protected IElementProgressService ElementProgressService { get; }
    protected INotebookRunService NotebookRunService { get; }
    protected IPageRunService PageRunService { get; }

    public DisplayHub(
        ILoggerFactory loggerFactory,
        IHubWrappers hubWrappers,
        // CoreHub (Framework)
        IAccessDeniedService accessDeniedService,
        IAccountSettingsService accountSettingsService,
        IAddressService addressService,
        IAuthService authService,
        IGatewayService gatewayService,
        ILinkClickService linkClickService,
        IMediaPlayService mediaPlayService,
        IPaneService paneService,
        IPortalRunService portalRunService,
        IPortalService portalService,
        ISegmentService segmentService,
        IServerConfigService serverConfigService,
        ISessionFetcher sessionFetcher,
        ISessionStateService sessionStateService,
        // DisplayHub (Content)
        IBinderRunService binderRunService,
        IBlogRunService blogRunService,
        ICacheService cacheService,
        IContactAchievementService contactAchievementService,
        IContentPortalRunService contentPortalRunService,
        ICourseRunService courseRunService,
        IElementProgressService elementProgressService,
        INotebookRunService notebookRunService,
        IPageRunService pageRunService)
        : base(loggerFactory,
            hubWrappers,
            accessDeniedService,
            accountSettingsService,
            addressService,
            authService,
            gatewayService,
            linkClickService,
            mediaPlayService,
            paneService,
            portalRunService,
            portalService,
            segmentService,
            serverConfigService,
            sessionFetcher,
            sessionStateService)
    {
        BinderRunService = binderRunService;
        BlogRunService = blogRunService;
        CacheService = cacheService;
        ContactAchievementService = contactAchievementService;
        ContentPortalRunService = contentPortalRunService;
        CourseRunService = courseRunService;
        ElementProgressService = elementProgressService;
        NotebookRunService = notebookRunService;
        PageRunService = pageRunService;
    }
}