namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

using License = Shared.Contracts.Data.License;
using BlogAdded = Crudspa.Content.Design.Shared.Contracts.Events.BlogAdded;
using BlogSaved = Crudspa.Content.Design.Shared.Contracts.Events.BlogSaved;
using BlogRemoved = Crudspa.Content.Design.Shared.Contracts.Events.BlogRemoved;

public partial class BlogLicenseManyForLicense : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IBlogLicenseService BlogLicenseService { get; set; } = null!;

    public BlogLicenseManyForLicenseModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, BlogLicenseService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class BlogLicenseManyForLicenseModel : ManyModel<BlogLicenseModel>,
    IHandle<BlogLicenseAdded>, IHandle<BlogLicenseSaved>, IHandle<BlogLicenseRemoved>,
    IHandle<BlogAdded>, IHandle<BlogSaved>, IHandle<BlogRemoved>
{
    private readonly IBlogLicenseService _blogLicenseService;
    private readonly Guid? _licenseId;

    public BlogLicenseManyForLicenseModel(IEventBus eventBus, IScrollService scrollService, IBlogLicenseService blogLicenseService, Guid? licenseId)
        : base(scrollService)
    {
        _blogLicenseService = blogLicenseService;

        _licenseId = licenseId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(BlogLicenseAdded payload) => await Replace(payload.Id, payload.LicenseId);

    public async Task Handle(BlogLicenseSaved payload) => await Replace(payload.Id, payload.LicenseId);

    public async Task Handle(BlogLicenseRemoved payload) => await Rid(payload.Id, payload.LicenseId);

    public async Task Handle(BlogAdded payload) => await FetchBlogNames();

    public async Task Handle(BlogSaved payload) => await FetchBlogNames();

    public async Task Handle(BlogRemoved payload) => await FetchBlogNames();

    public ObservableCollection<Named> BlogNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchBlogNames());

        await Refresh();
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request<License>(new() { Id = _licenseId });
        var response = await WithWaiting("Fetching...", () => _blogLicenseService.FetchForLicense(request), resetAlerts);

        if (response.Ok)
            SetForms(response.Value.Select(x => new BlogLicenseModel(x)));
    }

    public override async Task Create()
    {
        var blogLicense = new BlogLicense
        {
            Id = Guid.NewGuid(),
            BlogId = BlogNames.FirstOrDefault()?.Id,
            LicenseId = _licenseId,
        };

        var form = await CreateForm(new(blogLicense));
    }

    public override Boolean InScope(Guid? scopeId)
    {
        return scopeId is null || scopeId.Equals(_licenseId);
    }

    public override async Task<Response<BlogLicenseModel?>> Fetch(Guid? id)
    {
        var response = await _blogLicenseService.Fetch(new(new() { Id = id }));

        return response.Ok
            ? new(new BlogLicenseModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response<BlogLicenseModel?>> Add(FormModel<BlogLicenseModel> form)
    {
        var response = await _blogLicenseService.Add(new(form.Entity.BlogLicense));

        return response.Ok
            ? new(new BlogLicenseModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response> Save(FormModel<BlogLicenseModel> form)
    {
        var blogLicense = form.Entity.BlogLicense;

        return await _blogLicenseService.Save(new(blogLicense));
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _blogLicenseService.Remove(new(new()
        {
            Id = id,
            LicenseId = _licenseId,
        }));
    }

    public async Task FetchBlogNames()
    {
        var response = await WithAlerts(() => _blogLicenseService.FetchBlogNames(new()), false);
        if (response.Ok) BlogNames = response.Value.ToObservable();
    }
}

public class BlogLicenseModel : Observable, IDisposable, INamed
{
    private void HandleBlogLicenseChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(BlogLicense));

    private BlogLicense _blogLicense;

    public String? Name => BlogLicense.BlogTitle;

    public BlogLicenseModel(BlogLicense blogLicense)
    {
        _blogLicense = blogLicense;
        _blogLicense.PropertyChanged += HandleBlogLicenseChanged;
    }

    public void Dispose()
    {
        _blogLicense.PropertyChanged -= HandleBlogLicenseChanged;
    }

    public Guid? Id
    {
        get => _blogLicense.Id;
        set => _blogLicense.Id = value;
    }

    public BlogLicense BlogLicense
    {
        get => _blogLicense;
        set => SetProperty(ref _blogLicense, value);
    }
}