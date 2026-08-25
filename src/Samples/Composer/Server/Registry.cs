using Crudspa.Content.Design.Server.Repositories;
using Crudspa.Framework.Jobs.Server.Services;
using Crudspa.Samples.Composer.Server.Services;

namespace Crudspa.Samples.Composer.Server;

public class Registry
{
    public static void RegisterServices(IServiceCollection services, ServerConfig config)
    {
        // Crudspa.Content.Design.Server
        services.AddSingleton<IBinderRepository, BinderRepositorySql>();
        services.AddSingleton<IElementRepositoryFactory, ElementRepositoryFactoryContent>();
        services.AddSingleton<IPagePartsService, PagePartsServiceSql>();
        services.AddSingleton<IPageRepository, PageRepositorySql>();
        services.AddSingleton<ISectionRepository, SectionRepositorySql>();

        // Crudspa.Content.Design.Shared
        services.AddSingleton<IAchievementService, AchievementServiceSql>();
        services.AddSingleton<IBlogService, BlogServiceSql>();
        services.AddSingleton<IContainerService, ContainerServiceSql>();
        services.AddSingleton<ICommentService, CommentServiceSql>();
        services.AddSingleton<IContentPortalService, ContentPortalServiceSql>();
        services.AddSingleton<ICourseService, CourseServiceSql>();
        services.AddSingleton<IFontFaceService, FontFaceServiceSql>();
        services.AddSingleton<IFontService, FontServiceSql>();
        services.AddSingleton<IForumService, ForumServiceSql>();
        services.AddSingleton<IItemService, ItemServiceSql>();
        services.AddSingleton<IPanePageService, PanePageServiceSql>();
        services.AddSingleton<IPostService, PostServiceSql>();
        services.AddSingleton<ISectionService, SectionServiceSql>();
        services.AddSingleton<IStyleService, StyleServiceSql>();
        services.AddSingleton<IThreadService, ThreadServiceSql>();
        services.AddSingleton<ISurveyService, SurveyServiceSql>();
        services.AddSingleton<ITrackService, TrackServiceSql>();

        // Crudspa.Content.Display.Shared
        services.AddSingleton<IBinderRunService, BinderRunServiceSql>();
        services.AddSingleton<IBlogRunService, BlogRunServiceSql>();
        services.AddSingleton<IContactAchievementService, ContactAchievementServiceSql>();
        services.AddSingleton<IContentPortalRunService, ContentPortalRunServiceSql>();
        services.AddSingleton<ICourseRunService, CourseRunServiceSql>();
        services.AddSingleton<IElementProgressService, ElementProgressServiceSql>();
        services.AddSingleton<IForumMediaService, ForumMediaServiceSql>();
        services.AddSingleton<IForumRunService, ForumRunServiceSql>();
        services.AddSingleton<ISessionLicenseResolver, SessionLicenseResolverNone>();
        services.AddSingleton<INotebookRunService, NotebookRunServiceSql>();
        services.AddSingleton<IPageRunService, PageRunServiceContent>();
        services.AddSingleton<ISurveyRunService, SurveyRunServiceSql>();
        services.AddSingleton<IStylesRunService, StylesRunServiceSql>();
        services.AddSingleton<IThemeRunService, ThemeRunServiceSql>();

        // Crudspa.Content.Messaging.Shared
        services.AddSingleton<IActivationService, ActivationServiceSql>();
        services.AddSingleton<ICampaignService, CampaignServiceSql>();
        services.AddSingleton<IEmailService, EmailServiceSql>();
        services.AddSingleton<IEmailTemplateService, EmailTemplateServiceSql>();
        services.AddSingleton<IMemberService, MemberServiceSql>();
        services.AddSingleton<IMembershipService, MembershipServiceSql>();
        services.AddSingleton<IMessageService, MessageServiceSql>();
        services.AddSingleton<IPopulationResolver, OrganizationUsersPopulationResolver>();
        services.AddSingleton<IPopulationResolver, StaticMembershipPopulationResolver>();
        services.AddSingleton<IPopulationService, PopulationServiceSql>();
        services.AddSingleton<ISmsEventService, SmsEventServiceSql>();
        services.AddSingleton<ISmsMessageMediaService, SmsMessageMediaServiceSql>();
        services.AddSingleton<ISmsMessageService, SmsMessageServiceSql>();
        services.AddSingleton<ISmsPreferenceService, SmsPreferenceServiceSql>();
        services.AddSingleton<ISmsService, SmsServiceSql>();
        services.AddSingleton<ISmsTemplateService, SmsTemplateServiceSql>();
        services.AddSingleton<IStageService, StageServiceSql>();
        services.AddSingleton<ITokenService, TokenServiceSql>();

        // Crudspa.Samples.Composer.Shared
        services.AddSingleton<IComposerContactService, ComposerContactServiceSql>();
        services.AddSingleton<IComposerService, ComposerServiceSql>();

        // Crudspa.Framework.Core.Server
        services.AddByTypeName<IBlobService>(config.BlobService);
        services.AddByTypeName<IEmailSender>(config.EmailSender);
        services.AddSingleton<IAccessCodeService, AccessCodeServiceSql>();
        services.AddSingleton<IAccessDeniedService, AccessDeniedServiceSql>();
        services.AddSingleton<IAudioFileService, AudioFileServiceSql>();
        services.AddSingleton<IControllerWrappers, ControllerWrappersCore>();
        services.AddSingleton<ICryptographyService, CryptographyServiceCore>();
        services.AddSingleton<ICssInliner, CssInlinerPreMailer>();
        services.AddSingleton<ICssService, CssServiceCore>();
        services.AddSingleton<IEmailLayoutService, EmailLayoutServiceCore>();
        services.AddSingleton<IEmbeddedResourceService, EmbeddedResourceServiceCore>();
        services.AddSingleton<IFileService, FileServiceSql>();
        services.AddSingleton<IFontFileService, FontFileServiceSql>();
        services.AddSingleton<IGatewayService, GatewayServiceEventGrid>();
        services.AddSingleton<IHtmlSanitizer, HtmlSanitizerCore>();
        services.AddSingleton<IHubWrappers, HubWrappersCore>();
        services.AddSingleton<IImageFileService, ImageFileServiceSql>();
        services.AddSingleton<IPdfFileService, PdfFileServiceSql>();
        services.AddSingleton<ISassCompiler, SassCompilerDartSass>();
        services.AddSingleton<ISegmentFetcher, SegmentFetcherSql>();
        services.AddSingleton<IServerConfigService, ServerConfigServiceCore>();
        services.AddSingleton<IServiceWrappers, ServiceWrappersCore>();
        services.AddSingleton<ISessionFetcher, SessionFetcherCache>();
        services.AddSingleton<ISessionService, SessionServiceSql>();
        services.AddSingleton<ISessionWrappers, SessionWrappersCore>();
        services.AddSingleton<ISmsChannelService, SmsChannelServiceCore>();
        services.AddSingleton<ISmsSender, SmsSenderChannel>();
        services.AddSingleton<ISmsWebhookNotificationService, SmsWebhookNotificationServiceGateway>();
        services.AddSingleton<ISmsWebhookService, SmsWebhookServiceChannel>();
        services.AddSingleton<ISqlWrappers, SqlWrappersCore>();
        services.AddSingleton<IVideoFileService, VideoFileServiceSql>();
        services.AddSingleton<SmsSenderTwilio>();
        services.AddSingleton<SmsSenderNull>();
        services.AddSingleton<SmsSenderLocalFile>();
        services.AddSingleton<SmsWebhookServiceNull>();
        services.AddSingleton<SmsWebhookServiceLocalFile>();
        services.AddSingleton<TwilioSmsWebhookService>();

        // Crudspa.Framework.Core.Shared
        services.AddSingleton<IAccountSettingsService, AccountSettingsServiceSql>();
        services.AddSingleton<IAddressService, AddressServiceSql>();
        services.AddSingleton<IAuthService, AuthServiceSqlEmailTfa>();
        services.AddSingleton<ICacheService, CacheServiceMemory>();
        services.AddSingleton<IContactRepository, ContactRepositorySql>();
        services.AddSingleton<ILinkClickService, LinkClickServiceSql>();
        services.AddSingleton<IMediaPlayService, MediaPlayServiceSql>();
        services.AddSingleton<IOrganizationRepository, OrganizationRepositorySql>();
        services.AddSingleton<IPaneService, PaneServiceSql>();
        services.AddSingleton<IPortalRunService, PortalRunServiceSql>();
        services.AddSingleton<IPortalService, PortalServiceSql>();
        services.AddSingleton<ISegmentService, SegmentServiceSql>();
        services.AddSingleton<ISessionStateService, SessionStateServiceSql>();
        services.AddSingleton<IUserRepository, UserRepositorySql>();

        // Crudspa.Framework.Jobs.Shared
        services.AddSingleton<IJobScheduleService, JobScheduleServiceSql>();
        services.AddSingleton<IJobService, JobServiceSql>();

    }
}