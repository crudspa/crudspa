namespace Crudspa.Education.District.Server.Hubs;

public partial class DistrictHub : Crudspa.Content.Display.Server.Hubs.DisplayHub
{
    protected IActivityMediaPlayService ActivityMediaPlayService { get; }
    protected IActivityRunService ActivityRunService { get; }
    protected IAssessmentAssignmentService AssessmentAssignmentService { get; }
    protected ICommunityService CommunityService { get; }
    protected IDistrictContactService DistrictContactService { get; }
    protected IDistrictService DistrictService { get; }
    protected IReportService ReportService { get; }
    protected ISchoolContactService SchoolContactService { get; }
    protected ISchoolService SchoolService { get; }

    public DistrictHub(
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
        // DistrictHub (Education)
        IActivityMediaPlayService activityMediaPlayService,
        IActivityRunService activityRunService,
        IAssessmentAssignmentService assessmentAssignmentService,
        ICommunityService communityService,
        IDistrictContactService districtContactService,
        IDistrictService districtService,
        IReportService reportService,
        ISchoolContactService schoolContactService,
        ISchoolService schoolService)
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
        CommunityService = communityService;
        DistrictContactService = districtContactService;
        DistrictService = districtService;
        ReportService = reportService;
        SchoolContactService = schoolContactService;
        SchoolService = schoolService;
    }
}