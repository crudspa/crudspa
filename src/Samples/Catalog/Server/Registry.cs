using Crudspa.Framework.Jobs.Server.Services;
using Crudspa.Samples.Catalog.Server.Services;

namespace Crudspa.Samples.Catalog.Server;

public class Registry
{
    public static void RegisterServices(IServiceCollection services, ServerConfig config)
    {
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
        services.AddSingleton<ISqlWrappers, SqlWrappersCore>();
        services.AddSingleton<IVideoFileService, VideoFileServiceSql>();

        // Crudspa.Framework.Core.Shared
        services.AddSingleton<IAccountSettingsService, AccountSettingsServiceSql>();
        services.AddSingleton<IAddressService, AddressServiceSql>();
        services.AddSingleton<IAuthService, AuthServiceSqlCatalog>();
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

        // Crudspa.Samples.Catalog.Shared
        services.AddSingleton<IBookService, BookServiceSql>();
        services.AddSingleton<ICatalogContactService, CatalogContactServiceSql>();
        services.AddSingleton<ICatalogService, CatalogServiceSql>();
        services.AddSingleton<IMovieCreditService, MovieCreditServiceSql>();
        services.AddSingleton<IMovieService, MovieServiceSql>();
        services.AddSingleton<IShirtOptionService, ShirtOptionServiceSql>();
        services.AddSingleton<IShirtService, ShirtServiceSql>();
    }
}