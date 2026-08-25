
namespace Crudspa.Content.Design.Server.Hubs;

public partial class DesignHub : Crudspa.Content.Messaging.Server.Hubs.MessagingHub
{
    protected IAchievementService AchievementService { get; }
    protected IBlogService BlogService { get; }
    protected ICommentService CommentService { get; }
    protected IContainerService ContainerService { get; }
    protected IContentPortalService ContentPortalService { get; }
    protected ICourseService CourseService { get; }
    protected IFontFaceService FontFaceService { get; }
    protected IFontService FontService { get; }
    protected IForumService ForumService { get; }
    protected IItemService ItemService { get; }
    protected IPanePageService PanePageService { get; }
    protected IPostService PostService { get; }
    protected ISectionService SectionService { get; }
    protected IStyleService StyleService { get; }
    protected ISurveyService SurveyService { get; }
    protected IThreadService ThreadService { get; }
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
        IForumRunService forumRunService,
        INotebookRunService notebookRunService,
        IPageRunService pageRunService,
        ISurveyRunService surveyRunService,
        // MessagingHub (Content)
        IActivationService activationService,
        ICampaignService campaignService,
        IEmailService emailService,
        IEmailTemplateService emailTemplateService,
        IMemberService memberService,
        IMembershipService membershipService,
        IMessageService messageService,
        IPopulationService populationService,
        ISmsEventService smsEventService,
        ISmsMessageMediaService smsMessageMediaService,
        ISmsMessageService smsMessageService,
        ISmsPreferenceService smsPreferenceService,
        ISmsService smsService,
        ISmsTemplateService smsTemplateService,
        IStageService stageService,
        ITokenService tokenService,
        // DesignHub (Content)
        IAchievementService achievementService,
        IBlogService blogService,
        ICommentService commentService,
        IContainerService containerService,
        IContentPortalService contentPortalService,
        ICourseService courseService,
        IFontFaceService fontFaceService,
        IFontService fontService,
        IForumService forumService,
        IItemService itemService,
        IPanePageService panePageService,
        IPostService postService,
        ISectionService sectionService,
        IStyleService styleService,
        ISurveyService surveyService,
        IThreadService threadService,
        ITrackService trackService
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
            forumRunService,
            notebookRunService,
            pageRunService,
            surveyRunService,
            activationService,
            campaignService,
            emailService,
            emailTemplateService,
            memberService,
            membershipService,
            messageService,
            populationService,
            smsEventService,
            smsMessageMediaService,
            smsMessageService,
            smsPreferenceService,
            smsService,
            smsTemplateService,
            stageService,
            tokenService
            )
    {
        AchievementService = achievementService;
        BlogService = blogService;
        CommentService = commentService;
        ContainerService = containerService;
        ContentPortalService = contentPortalService;
        CourseService = courseService;
        FontFaceService = fontFaceService;
        FontService = fontService;
        ForumService = forumService;
        ItemService = itemService;
        PanePageService = panePageService;
        PostService = postService;
        SectionService = sectionService;
        StyleService = styleService;
        SurveyService = surveyService;
        ThreadService = threadService;
        TrackService = trackService;
    }
}