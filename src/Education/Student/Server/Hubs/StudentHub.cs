namespace Crudspa.Education.Student.Server.Hubs;

public partial class StudentHub : Crudspa.Content.Display.Server.Hubs.DisplayHub
{
    protected IActivityMediaPlayService ActivityMediaPlayService { get; }
    protected IActivityRunService ActivityRunService { get; }
    protected IAssessmentRunService AssessmentRunService { get; }
    protected IBookProgressService BookProgressService { get; }
    protected IChapterProgressService ChapterProgressService { get; }
    protected IGameProgressService GameProgressService { get; }
    protected IModuleProgressService ModuleProgressService { get; }
    protected IObjectiveProgressService ObjectiveProgressService { get; }
    protected IStudentAchievementService StudentAchievementService { get; }
    protected IStudentAppService StudentAppService { get; }
    protected ITrifoldProgressService TrifoldProgressService { get; }

    public StudentHub(
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
        // StudentHub (Education)
        IActivityMediaPlayService activityMediaPlayService,
        IActivityRunService activityRunService,
        IAssessmentRunService assessmentRunService,
        IBookProgressService bookProgressService,
        IChapterProgressService chapterProgressService,
        IGameProgressService gameProgressService,
        IModuleProgressService moduleProgressService,
        IObjectiveProgressService objectiveProgressService,
        IStudentAchievementService studentAchievementService,
        IStudentAppService studentAppService,
        ITrifoldProgressService trifoldProgressService)
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
        ActivityMediaPlayService = activityMediaPlayService;
        ActivityRunService = activityRunService;
        AssessmentRunService = assessmentRunService;
        BookProgressService = bookProgressService;
        ChapterProgressService = chapterProgressService;
        GameProgressService = gameProgressService;
        ModuleProgressService = moduleProgressService;
        ObjectiveProgressService = objectiveProgressService;
        StudentAchievementService = studentAchievementService;
        StudentAppService = studentAppService;
        TrifoldProgressService = trifoldProgressService;
    }
}