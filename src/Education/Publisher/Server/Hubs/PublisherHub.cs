using IAchievementService = Crudspa.Content.Design.Shared.Contracts.Behavior.IAchievementService;
using IForumService = Crudspa.Content.Design.Shared.Contracts.Behavior.IForumService;
using IPostService = Crudspa.Content.Design.Shared.Contracts.Behavior.IPostService;

namespace Crudspa.Education.Publisher.Server.Hubs;

public partial class PublisherHub : Crudspa.Content.Design.Server.Hubs.DesignHub
{
    protected Shared.Contracts.Behavior.IAchievementService EducationAchievementService { get; }
    protected IActivityElementService ActivityElementService { get; }
    protected IActivityMediaPlayService ActivityMediaPlayService { get; }
    protected IActivityRunService ActivityRunService { get; }
    protected IActivityService ActivityService { get; }
    protected IAssessmentService AssessmentService { get; }
    protected IBookService BookService { get; }
    protected IChapterService ChapterService { get; }
    protected IClassRecordingService ClassRecordingService { get; }
    protected ICommunityService CommunityService { get; }
    protected IDistrictContactService DistrictContactService { get; }
    protected IDistrictLicenseService DistrictLicenseService { get; }
    protected IDistrictService DistrictService { get; }
    protected Shared.Contracts.Behavior.IForumService PublisherForumService { get; }
    protected IGameActivityService GameActivityService { get; }
    protected IGameSectionService GameSectionService { get; }
    protected IGameService GameService { get; }
    protected ILessonService LessonService { get; }
    protected ILicenseService LicenseService { get; }
    protected IListenPartService ListenPartService { get; }
    protected IListenQuestionService ListenQuestionService { get; }
    protected IModuleService ModuleService { get; }
    protected IObjectiveService ObjectiveService { get; }
    protected Shared.Contracts.Behavior.IPostService PublisherPostService { get; }
    protected IPublisherContactService PublisherContactService { get; }
    protected IPublisherService PublisherService { get; }
    protected IReadParagraphService ReadParagraphService { get; }
    protected IReadPartService ReadPartService { get; }
    protected IReadQuestionService ReadQuestionService { get; }
    protected ISchoolContactService SchoolContactService { get; }
    protected ISchoolService SchoolService { get; }
    protected ISchoolYearService SchoolYearService { get; }
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
        // PublisherHub (Education)
        Shared.Contracts.Behavior.IAchievementService educationAchievementService,
        IActivityElementService activityElementService,
        IActivityMediaPlayService activityMediaPlayService,
        IActivityRunService activityRunService,
        IActivityService activityService,
        IAssessmentService assessmentService,
        IBookService bookService,
        IChapterService chapterService,
        IClassRecordingService classRecordingService,
        ICommunityService communityService,
        IDistrictContactService districtContactService,
        IDistrictLicenseService districtLicenseService,
        IDistrictService districtService,
        Shared.Contracts.Behavior.IForumService publisherForumService,
        IGameActivityService gameActivityService,
        IGameSectionService gameSectionService,
        IGameService gameService,
        ILessonService lessonService,
        ILicenseService licenseService,
        IListenPartService listenPartService,
        IListenQuestionService listenQuestionService,
        IModuleService moduleService,
        IObjectiveService objectiveService,
        Shared.Contracts.Behavior.IPostService publisherPostService,
        IPublisherContactService publisherContactService,
        IPublisherService publisherService,
        IReadParagraphService readParagraphService,
        IReadPartService readPartService,
        IReadQuestionService readQuestionService,
        ISchoolContactService schoolContactService,
        ISchoolService schoolService,
        ISchoolYearService schoolYearService,
        ITrifoldService trifoldService,
        IUnitBookService unitBookService,
        IUnitLicenseService unitLicenseService,
        IUnitService unitService,
        IVocabPartService vocabPartService,
        IVocabQuestionService vocabQuestionService)
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
            trackService)
    {
        EducationAchievementService = educationAchievementService;
        ActivityElementService = activityElementService;
        ActivityMediaPlayService = activityMediaPlayService;
        ActivityRunService = activityRunService;
        ActivityService = activityService;
        AssessmentService = assessmentService;
        BookService = bookService;
        ChapterService = chapterService;
        ClassRecordingService = classRecordingService;
        CommunityService = communityService;
        DistrictContactService = districtContactService;
        DistrictLicenseService = districtLicenseService;
        DistrictService = districtService;
        PublisherForumService = publisherForumService;
        GameActivityService = gameActivityService;
        GameSectionService = gameSectionService;
        GameService = gameService;
        LessonService = lessonService;
        LicenseService = licenseService;
        ListenPartService = listenPartService;
        ListenQuestionService = listenQuestionService;
        ModuleService = moduleService;
        ObjectiveService = objectiveService;
        PublisherPostService = publisherPostService;
        PublisherContactService = publisherContactService;
        PublisherService = publisherService;
        ReadParagraphService = readParagraphService;
        ReadPartService = readPartService;
        ReadQuestionService = readQuestionService;
        SchoolContactService = schoolContactService;
        SchoolService = schoolService;
        SchoolYearService = schoolYearService;
        TrifoldService = trifoldService;
        UnitBookService = unitBookService;
        UnitLicenseService = unitLicenseService;
        UnitService = unitService;
        VocabPartService = vocabPartService;
        VocabQuestionService = vocabQuestionService;
    }
}