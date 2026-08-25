
namespace Crudspa.Education.Provider.Server.Hubs;

public partial class ProviderHub : Crudspa.Content.Design.Server.Hubs.DesignHub
{
    protected IJobScheduleService JobScheduleService { get; }
    protected IJobService JobService { get; }
    protected IProviderContactService ProviderContactService { get; }
    protected IProviderService ProviderService { get; }
    protected IPublisherContactService PublisherContactService { get; }
    protected IPublisherService PublisherService { get; }

    public ProviderHub(
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
        ITrackService trackService,
        // ProviderHub (Education)
        IJobScheduleService jobScheduleService,
        IJobService jobService,
        IProviderContactService providerContactService,
        IProviderService providerService,
        IPublisherContactService publisherContactService,
        IPublisherService publisherService
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
            tokenService,
            achievementService,
            blogService,
            commentService,
            containerService,
            contentPortalService,
            courseService,
            fontFaceService,
            fontService,
            forumService,
            itemService,
            panePageService,
            postService,
            sectionService,
            styleService,
            surveyService,
            threadService,
            trackService
            )
    {
        JobScheduleService = jobScheduleService;
        JobService = jobService;
        ProviderContactService = providerContactService;
        ProviderService = providerService;
        PublisherContactService = publisherContactService;
        PublisherService = publisherService;
    }
}