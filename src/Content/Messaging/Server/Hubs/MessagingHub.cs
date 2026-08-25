
namespace Crudspa.Content.Messaging.Server.Hubs;

public partial class MessagingHub : Crudspa.Content.Display.Server.Hubs.DisplayHub
{
    protected IActivationService ActivationService { get; }
    protected ICampaignService CampaignService { get; }
    protected IEmailService EmailService { get; }
    protected IEmailTemplateService EmailTemplateService { get; }
    protected IMemberService MemberService { get; }
    protected IMembershipService MembershipService { get; }
    protected IMessageService MessageService { get; }
    protected IPopulationService PopulationService { get; }
    protected ISmsEventService SmsEventService { get; }
    protected ISmsMessageMediaService SmsMessageMediaService { get; }
    protected ISmsMessageService SmsMessageService { get; }
    protected ISmsPreferenceService SmsPreferenceService { get; }
    protected ISmsService SmsService { get; }
    protected ISmsTemplateService SmsTemplateService { get; }
    protected IStageService StageService { get; }
    protected ITokenService TokenService { get; }

    public MessagingHub(
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
        ITokenService tokenService
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
            surveyRunService
            )
    {
        ActivationService = activationService;
        CampaignService = campaignService;
        EmailService = emailService;
        EmailTemplateService = emailTemplateService;
        MemberService = memberService;
        MembershipService = membershipService;
        MessageService = messageService;
        PopulationService = populationService;
        SmsEventService = smsEventService;
        SmsMessageMediaService = smsMessageMediaService;
        SmsMessageService = smsMessageService;
        SmsPreferenceService = smsPreferenceService;
        SmsService = smsService;
        SmsTemplateService = smsTemplateService;
        StageService = stageService;
        TokenService = tokenService;
    }
}