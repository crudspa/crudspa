
namespace Crudspa.Education.District.Server.Hubs;

public partial class DistrictHub : Crudspa.Content.Messaging.Server.Hubs.MessagingHub
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
        // DistrictHub (Education)
        IActivityMediaPlayService activityMediaPlayService,
        IActivityRunService activityRunService,
        IAssessmentAssignmentService assessmentAssignmentService,
        ICommunityService communityService,
        IDistrictContactService districtContactService,
        IDistrictService districtService,
        IReportService reportService,
        ISchoolContactService schoolContactService,
        ISchoolService schoolService
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