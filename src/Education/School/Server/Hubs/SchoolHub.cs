namespace Crudspa.Education.School.Server.Hubs;

public partial class SchoolHub : Crudspa.Content.Display.Server.Hubs.DisplayHub
{
    protected IActivityMediaPlayService ActivityMediaPlayService { get; }
    protected IActivityRunService ActivityRunService { get; }
    protected IAssessmentAssignmentService AssessmentAssignmentService { get; }
    protected IClassRecordingService ClassRecordingService { get; }
    protected IClassroomService ClassroomService { get; }
    protected Shared.Contracts.Behavior.IPostService SchoolPostService { get; }
    protected IReportService ReportService { get; }
    protected ISchoolContactService SchoolContactService { get; }
    protected ISchoolService SchoolService { get; }
    protected ISchoolYearService SchoolYearService { get; }
    protected IStudentService StudentService { get; }

    public SchoolHub(
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
        // SchoolHub (Education)
        IActivityMediaPlayService activityMediaPlayService,
        IActivityRunService activityRunService,
        IAssessmentAssignmentService assessmentAssignmentService,
        IClassRecordingService classRecordingService,
        IClassroomService classroomService,
        Shared.Contracts.Behavior.IPostService schoolPostService,
        IReportService reportService,
        ISchoolContactService schoolContactService,
        ISchoolService schoolService,
        ISchoolYearService schoolYearService,
        IStudentService studentService)
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
        AssessmentAssignmentService = assessmentAssignmentService;
        ClassRecordingService = classRecordingService;
        ClassroomService = classroomService;
        SchoolPostService = schoolPostService;
        ReportService = reportService;
        SchoolContactService = schoolContactService;
        SchoolService = schoolService;
        SchoolYearService = schoolYearService;
        StudentService = studentService;
    }
}