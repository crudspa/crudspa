namespace Crudspa.Content.Design.Server.Hubs;

public partial class DesignHub : Crudspa.Content.Display.Server.Hubs.DisplayHub
{
    protected IAchievementService AchievementService { get; }
    protected IBlogService BlogService { get; }
    protected IContainerService ContainerService { get; }
    protected IContentPortalService ContentPortalService { get; }
    protected ICourseService CourseService { get; }
    protected IEmailService EmailService { get; }
    protected IEmailTemplateService EmailTemplateService { get; }
    protected IFontService FontService { get; }
    protected IForumService ForumService { get; }
    protected IItemService ItemService { get; }
    protected IMemberService MemberService { get; }
    protected IMembershipService MembershipService { get; }
    protected IPanePageService PanePageService { get; }
    protected IPostService PostService { get; }
    protected ISectionService SectionService { get; }
    protected IStyleService StyleService { get; }
    protected IThreadService ThreadService { get; }
    protected ITokenService TokenService { get; }
    protected ITrackService TrackService { get; }

    public DesignHub(
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
        ITrackService trackService)
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
            pageRunService)
    {
        AchievementService = achievementService;
        BlogService = blogService;
        ContainerService = containerService;
        ContentPortalService = contentPortalService;
        CourseService = courseService;
        EmailService = emailService;
        EmailTemplateService = emailTemplateService;
        FontService = fontService;
        ForumService = forumService;
        ItemService = itemService;
        MemberService = memberService;
        MembershipService = membershipService;
        PanePageService = panePageService;
        PostService = postService;
        SectionService = sectionService;
        StyleService = styleService;
        ThreadService = threadService;
        TokenService = tokenService;
        TrackService = trackService;
    }
}