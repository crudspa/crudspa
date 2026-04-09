
namespace Crudspa.Samples.Composer.Server.Hubs;

public partial class ComposerHub : Crudspa.Content.Design.Server.Hubs.DesignHub
{
    protected IJobScheduleService JobScheduleService { get; }
    protected IJobService JobService { get; }
    protected IComposerContactService ComposerContactService { get; }
    protected IComposerService ComposerService { get; }

    public ComposerHub(
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
        IPageRunService pageRunService,
        // DesignHub (Content)
        IAchievementService achievementService,
        IBlogService blogService,
        IContainerService containerService,
        IContentPortalService contentPortalService,
        ICourseService courseService,
        IEmailService emailService,
        IEmailTemplateService emailTemplateService,
        IFontService fontService,
        IForumService forumService,
        IItemService itemService,
        IMemberService memberService,
        IMembershipService membershipService,
        IPanePageService panePageService,
        IPostService postService,
        ISectionService sectionService,
        IStyleService styleService,
        IThreadService threadService,
        ITokenService tokenService,
        ITrackService trackService,
        IJobScheduleService jobScheduleService,
        IJobService jobService,
        IComposerService composerService,
        IComposerContactService composerContactService
        )
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
            sessionStateService,
            binderRunService,
            blogRunService,
            cacheService,
            contactAchievementService,
            contentPortalRunService,
            courseRunService,
            elementProgressService,
            notebookRunService,
            pageRunService,
            achievementService,
            blogService,
            containerService,
            contentPortalService,
            courseService,
            emailService,
            emailTemplateService,
            fontService,
            forumService,
            itemService,
            memberService,
            membershipService,
            panePageService,
            postService,
            sectionService,
            styleService,
            threadService,
            tokenService,
            trackService
            )
    {
        JobScheduleService = jobScheduleService;
        JobService = jobService;
        ComposerService = composerService;
        ComposerContactService = composerContactService;
    }
}