using IAchievementService = Crudspa.Content.Design.Shared.Contracts.Behavior.IAchievementService;
using IForumService = Crudspa.Content.Design.Shared.Contracts.Behavior.IForumService;
using IPostService = Crudspa.Content.Design.Shared.Contracts.Behavior.IPostService;

namespace Crudspa.Education.Publisher.Server.Hubs;

public partial class PublisherHub : Crudspa.Content.Design.Server.Hubs.DesignHub
{
    protected Crudspa.Education.Publisher.Shared.Contracts.Behavior.IAchievementService EducationAchievementService { get; }
    protected IActivityElementService ActivityElementService { get; }
    protected IActivityMediaPlayService ActivityMediaPlayService { get; }
    protected IActivityRunService ActivityRunService { get; }
    protected IActivityService ActivityService { get; }
    protected IAssessmentLicenseService AssessmentLicenseService { get; }
    protected IAssessmentService AssessmentService { get; }
    protected IBlogLicenseService BlogLicenseService { get; }
    protected IBookService BookService { get; }
    protected IChapterService ChapterService { get; }
    protected ICampaignLicenseService CampaignLicenseService { get; }
    protected IClassRecordingService ClassRecordingService { get; }
    protected ICommunityService CommunityService { get; }
    protected IDistrictContactService DistrictContactService { get; }
    protected IDistrictLicenseService DistrictLicenseService { get; }
    protected IDistrictService DistrictService { get; }
    protected IForumLicenseService ForumLicenseService { get; }
    protected IGameActivityService GameActivityService { get; }
    protected IGameSectionService GameSectionService { get; }
    protected IGameService GameService { get; }
    protected ILessonService LessonService { get; }
    protected ILicenseService LicenseService { get; }
    protected IListenPartService ListenPartService { get; }
    protected IListenQuestionService ListenQuestionService { get; }
    protected IModuleService ModuleService { get; }
    protected IObjectiveService ObjectiveService { get; }
    protected IPublisherContactService PublisherContactService { get; }
    protected IPublisherService PublisherService { get; }
    protected IReadParagraphService ReadParagraphService { get; }
    protected IReadPartService ReadPartService { get; }
    protected IReadQuestionService ReadQuestionService { get; }
    protected ISchoolContactService SchoolContactService { get; }
    protected ISchoolService SchoolService { get; }
    protected ISchoolYearService SchoolYearService { get; }
    protected ISegmentLicenseService SegmentLicenseService { get; }
    protected ISurveyLicenseService SurveyLicenseService { get; }
    protected ITrackLicenseService TrackLicenseService { get; }
    protected ITrifoldService TrifoldService { get; }
    protected IUnitBookService UnitBookService { get; }
    protected IUnitLicenseService UnitLicenseService { get; }
    protected IUnitService UnitService { get; }
    protected IVocabPartService VocabPartService { get; }
    protected IVocabQuestionService VocabQuestionService { get; }

    public PublisherHub(
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
        // PublisherHub (Education)
        Crudspa.Education.Publisher.Shared.Contracts.Behavior.IAchievementService publisherAchievementService,
        IActivityElementService activityElementService,
        IActivityMediaPlayService activityMediaPlayService,
        IActivityRunService activityRunService,
        IActivityService activityService,
        IAssessmentLicenseService assessmentLicenseService,
        IAssessmentService assessmentService,
        IBlogLicenseService blogLicenseService,
        IBookService bookService,
        IChapterService chapterService,
        ICampaignLicenseService campaignLicenseService,
        IClassRecordingService classRecordingService,
        ICommunityService communityService,
        IDistrictContactService districtContactService,
        IDistrictLicenseService districtLicenseService,
        IDistrictService districtService,
        IForumLicenseService forumLicenseService,
        IGameActivityService gameActivityService,
        IGameSectionService gameSectionService,
        IGameService gameService,
        ILessonService lessonService,
        ILicenseService licenseService,
        IListenPartService listenPartService,
        IListenQuestionService listenQuestionService,
        IModuleService moduleService,
        IObjectiveService objectiveService,
        IPublisherContactService publisherContactService,
        IPublisherService publisherService,
        IReadParagraphService readParagraphService,
        IReadPartService readPartService,
        IReadQuestionService readQuestionService,
        ISchoolContactService schoolContactService,
        ISchoolService schoolService,
        ISchoolYearService schoolYearService,
        ISegmentLicenseService segmentLicenseService,
        ISurveyLicenseService surveyLicenseService,
        ITrackLicenseService trackLicenseService,
        ITrifoldService trifoldService,
        IUnitBookService unitBookService,
        IUnitLicenseService unitLicenseService,
        IUnitService unitService,
        IVocabPartService vocabPartService,
        IVocabQuestionService vocabQuestionService
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
        EducationAchievementService = publisherAchievementService;
        ActivityElementService = activityElementService;
        ActivityMediaPlayService = activityMediaPlayService;
        ActivityRunService = activityRunService;
        ActivityService = activityService;
        AssessmentLicenseService = assessmentLicenseService;
        AssessmentService = assessmentService;
        BlogLicenseService = blogLicenseService;
        BookService = bookService;
        ChapterService = chapterService;
        CampaignLicenseService = campaignLicenseService;
        ClassRecordingService = classRecordingService;
        CommunityService = communityService;
        DistrictContactService = districtContactService;
        DistrictLicenseService = districtLicenseService;
        DistrictService = districtService;
        ForumLicenseService = forumLicenseService;
        GameActivityService = gameActivityService;
        GameSectionService = gameSectionService;
        GameService = gameService;
        LessonService = lessonService;
        LicenseService = licenseService;
        ListenPartService = listenPartService;
        ListenQuestionService = listenQuestionService;
        ModuleService = moduleService;
        ObjectiveService = objectiveService;
        PublisherContactService = publisherContactService;
        PublisherService = publisherService;
        ReadParagraphService = readParagraphService;
        ReadPartService = readPartService;
        ReadQuestionService = readQuestionService;
        SchoolContactService = schoolContactService;
        SchoolService = schoolService;
        SchoolYearService = schoolYearService;
        SegmentLicenseService = segmentLicenseService;
        SurveyLicenseService = surveyLicenseService;
        TrackLicenseService = trackLicenseService;
        TrifoldService = trifoldService;
        UnitBookService = unitBookService;
        UnitLicenseService = unitLicenseService;
        UnitService = unitService;
        VocabPartService = vocabPartService;
        VocabQuestionService = vocabQuestionService;
    }
}