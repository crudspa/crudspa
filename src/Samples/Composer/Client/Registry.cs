using Crudspa.Content.Design.Client.Services;
using Crudspa.Content.Messaging.Client.Services;
using Crudspa.Framework.Jobs.Client.Services;
using Crudspa.Samples.Composer.Client.Services;

namespace Crudspa.Samples.Composer.Client;

public class Registry
{
    public static void RegisterServices(IServiceCollection services)
    {
        // Logging
        services.AddSingleton<ILoggerProvider, ClientLoggerProviderCore>();

        // Crudspa.Content.Design.Shared
        services.AddSingleton<IAchievementService, AchievementServiceTcp>();
        services.AddSingleton<IBlogService, BlogServiceTcp>();
        services.AddSingleton<IContainerService, ContainerServiceTcp>();
        services.AddSingleton<ICommentService, CommentServiceTcp>();
        services.AddSingleton<IContentPortalService, ContentPortalServiceTcp>();
        services.AddSingleton<ICourseService, CourseServiceTcp>();
        services.AddSingleton<IFontFaceService, FontFaceServiceTcp>();
        services.AddSingleton<IFontService, FontServiceTcp>();
        services.AddSingleton<IForumService, ForumServiceTcp>();
        services.AddSingleton<IItemService, ItemServiceTcp>();
        services.AddSingleton<IPanePageService, PanePageServiceTcp>();
        services.AddSingleton<IPostService, PostServiceTcp>();
        services.AddSingleton<ISectionService, SectionServiceTcp>();
        services.AddSingleton<IStyleService, StyleServiceTcp>();
        services.AddSingleton<IThreadService, ThreadServiceTcp>();
        services.AddSingleton<ISurveyService, SurveyServiceTcp>();
        services.AddSingleton<ITrackService, TrackServiceTcp>();

        // Crudspa.Content.Display.Shared
        services.AddSingleton<IBinderRunService, BinderRunServiceTcp>();
        services.AddSingleton<IBlogRunService, BlogRunServiceTcp>();
        services.AddSingleton<IContactAchievementService, ContactAchievementServiceTcp>();
        services.AddSingleton<IContentPortalRunService, ContentPortalRunServiceTcp>();
        services.AddSingleton<ICourseRunService, CourseRunServiceTcp>();
        services.AddSingleton<IElementProgressService, ElementProgressServiceTcp>();
        services.AddSingleton<IForumRunService, ForumRunServiceTcp>();
        services.AddSingleton<INotebookRunService, NotebookRunServiceTcp>();
        services.AddSingleton<IPageRunService, PageRunServiceTcp>();
        services.AddSingleton<ISurveyRunService, SurveyRunServiceTcp>();

        // Crudspa.Content.Messaging.Shared
        services.AddSingleton<IActivationService, ActivationServiceTcp>();
        services.AddSingleton<ICampaignService, CampaignServiceTcp>();
        services.AddSingleton<IEmailService, EmailServiceTcp>();
        services.AddSingleton<IEmailTemplateService, EmailTemplateServiceTcp>();
        services.AddSingleton<IMemberService, MemberServiceTcp>();
        services.AddSingleton<IMembershipService, MembershipServiceTcp>();
        services.AddSingleton<IMessageService, MessageServiceTcp>();
        services.AddSingleton<IPopulationService, PopulationServiceTcp>();
        services.AddSingleton<ISmsEventService, SmsEventServiceTcp>();
        services.AddSingleton<ISmsMessageMediaService, SmsMessageMediaServiceTcp>();
        services.AddSingleton<ISmsMessageService, SmsMessageServiceTcp>();
        services.AddSingleton<ISmsPreferenceService, SmsPreferenceServiceTcp>();
        services.AddSingleton<ISmsService, SmsServiceTcp>();
        services.AddSingleton<ISmsTemplateService, SmsTemplateServiceTcp>();
        services.AddSingleton<IStageService, StageServiceTcp>();
        services.AddSingleton<ITokenService, TokenServiceTcp>();

        // Crudspa.Framework.Core.Client
        services.AddSingleton<IClickService, ClickServiceCore>();
        services.AddSingleton<ICookieService, CookieServiceCore>();
        services.AddSingleton<IEventBus, EventBusCore>();
        services.AddSingleton<IJsBridge, JsBridgeCore>();
        services.AddSingleton<INavigator, NavigatorCore>();
        services.AddSingleton<IProxyWrappers, ProxyWrappersCore>();
        services.AddSingleton<IScrollService, ScrollServiceCore>();
        services.AddSingleton<ISessionState, SessionStateCore>();
        services.AddSingleton<IUriProvider, UriProviderCore>();

        // Crudspa.Framework.Core.Shared
        services.AddSingleton<IAccountSettingsService, AccountSettingsServiceTcp>();
        services.AddSingleton<IAddressService, AddressServiceTcp>();
        services.AddSingleton<IAuthService, AuthServiceTcpEmailTfa>();
        services.AddSingleton<ICacheService, CacheServiceDictionary>();
        services.AddSingleton<ILinkClickService, LinkClickServiceTcp>();
        services.AddSingleton<IMediaPlayService, MediaPlayServiceTcp>();
        services.AddSingleton<IPaneService, PaneServiceTcp>();
        services.AddSingleton<IPortalRunService, PortalRunServiceTcp>();
        services.AddSingleton<IPortalService, PortalServiceTcp>();
        services.AddSingleton<ISegmentService, SegmentServiceTcp>();
        services.AddSingleton<ISessionStateService, SessionStateServiceTcp>();

        // Crudspa.Framework.Jobs.Client
        services.AddSingleton<IJobCopyService, JobCopyServiceJobs>();

        // Crudspa.Framework.Jobs.Shared
        services.AddSingleton<IJobScheduleService, JobScheduleServiceTcp>();
        services.AddSingleton<IJobService, JobServiceTcp>();

        // Crudspa.Samples.Composer.Shared
        services.AddSingleton<IComposerContactService, ComposerContactServiceTcp>();
        services.AddSingleton<IComposerService, ComposerServiceTcp>();

    }
}