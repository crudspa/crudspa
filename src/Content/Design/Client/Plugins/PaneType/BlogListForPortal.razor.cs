namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class BlogListForPortal : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IBlogService BlogService { get; set; } = null!;

    public BlogListForPortalModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, BlogService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class BlogListForPortalModel : ListModel<BlogModel>,
    IHandle<BlogAdded>, IHandle<BlogSaved>, IHandle<BlogRemoved>,
    IHandle<PostAdded>, IHandle<PostRemoved>
{
    private readonly IBlogService _blogService;
    private readonly Guid? _portalId;

    public BlogListForPortalModel(IEventBus eventBus, IScrollService scrollService, IBlogService blogService, Guid? portalId)
        : base(scrollService)
    {
        _blogService = blogService;

        _portalId = portalId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(BlogAdded payload) => await Replace(payload.Id, payload.PortalId);

    public async Task Handle(BlogSaved payload) => await Replace(payload.Id, payload.PortalId);

    public async Task Handle(BlogRemoved payload) => await Rid(payload.Id, payload.PortalId);

    public async Task Handle(PostAdded payload)
    {
        if (payload.BlogId is not null && Cards.HasAny(x => x.Entity.Id.Equals(payload.BlogId)))
            await Replace(payload.BlogId);
    }

    public async Task Handle(PostRemoved payload)
    {
        if (payload.BlogId is not null && Cards.HasAny(x => x.Entity.Id.Equals(payload.BlogId)))
            await Replace(payload.BlogId);
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request<Portal>(new() { Id = _portalId });
        var response = await WithWaiting("Fetching...", () => _blogService.FetchForPortal(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value.Select(x => new BlogModel(x)).ToList());
    }

    public override async Task<Response<BlogModel?>> Fetch(Guid? id)
    {
        var response = await _blogService.Fetch(new(new() { Id = id }));

        return response.Ok
            ? new(new BlogModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _blogService.Remove(new(new()
        {
            Id = id,
            PortalId = _portalId,
        }));
    }

    public override Boolean InScope(Guid? scopeId)
    {
        return scopeId is null || scopeId.Equals(_portalId);
    }
}

public class BlogModel : Observable, IDisposable, INamed
{
    private void HandleBlogChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(Blog));

    private Blog _blog;

    public String? Name => Blog.Title;

    public BlogModel(Blog blog)
    {
        _blog = blog;
        _blog.PropertyChanged += HandleBlogChanged;
    }

    public void Dispose()
    {
        _blog.PropertyChanged -= HandleBlogChanged;
    }

    public Guid? Id
    {
        get => _blog.Id;
        set => _blog.Id = value;
    }

    public Blog Blog
    {
        get => _blog;
        set => SetProperty(ref _blog, value);
    }
}