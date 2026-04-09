
namespace Crudspa.Samples.Catalog.Server.Hubs;

public partial class CatalogHub : Crudspa.Framework.Core.Server.Hubs.CoreHub
{
    protected IBookService BookService { get; }
    protected ICatalogContactService CatalogContactService { get; }
    protected ICatalogService CatalogService { get; }
    protected IJobScheduleService JobScheduleService { get; }
    protected IJobService JobService { get; }
    protected IMovieCreditService MovieCreditService { get; }
    protected IMovieService MovieService { get; }
    protected IShirtOptionService ShirtOptionService { get; }
    protected IShirtService ShirtService { get; }

    public CatalogHub(
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
        // CatalogHub (Samples)
        IBookService bookService,
        ICatalogContactService catalogContactService,
        ICatalogService catalogService,
        IJobScheduleService jobScheduleService,
        IJobService jobService,
        IMovieCreditService movieCreditService,
        IMovieService movieService,
        IShirtOptionService shirtOptionService,
        IShirtService shirtService
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
            sessionStateService
            )
    {
        BookService = bookService;
        CatalogContactService = catalogContactService;
        CatalogService = catalogService;
        JobScheduleService = jobScheduleService;
        JobService = jobService;
        MovieCreditService = movieCreditService;
        MovieService = movieService;
        ShirtOptionService = shirtOptionService;
        ShirtService = shirtService;
    }
}