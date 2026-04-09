namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class FontManyForPortal : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IFontService FontService { get; set; } = null!;

    public FontManyForPortalModel Model { get; set; } = null!;
    public List<String> AllowedExtensions { get; set; } = [".otf", ".ttf", ".woff", ".woff2"];

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, FontService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class FontManyForPortalModel : ManyModel<FontModel>,
    IHandle<FontAdded>, IHandle<FontSaved>, IHandle<FontRemoved>
{
    private readonly IFontService _fontService;
    private readonly Guid? _contentPortalId;

    public FontManyForPortalModel(IEventBus eventBus, IScrollService scrollService, IFontService fontService, Guid? contentPortalId)
        : base(scrollService)
    {
        _fontService = fontService;
        _contentPortalId = contentPortalId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(FontAdded payload) => await Replace(payload.Id, payload.ContentPortalId);
    public async Task Handle(FontSaved payload) => await Replace(payload.Id, payload.ContentPortalId);
    public async Task Handle(FontRemoved payload) => await Rid(payload.Id, payload.ContentPortalId);

    public async Task Initialize()
    {
        await Refresh();
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request<ContentPortal>(new() { Id = _contentPortalId });
        var response = await WithWaiting("Fetching...", () => _fontService.FetchForContentPortal(request), resetAlerts);

        if (response.Ok)
            SetForms(response.Value.Select(x => new FontModel(x)));
    }

    public override async Task Create()
    {
        var font = new Font
        {
            Id = Guid.NewGuid(),
            ContentPortalId = _contentPortalId,
            Name = "New Font",
        };

        await CreateForm(new(font));
    }

    public override Boolean InScope(Guid? scopeId)
    {
        return scopeId is null || scopeId.Equals(_contentPortalId);
    }

    public override async Task<Response<FontModel?>> Fetch(Guid? id)
    {
        var response = await _fontService.Fetch(new(new() { Id = id }));

        return response.Ok
            ? new(new FontModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response<FontModel?>> Add(FormModel<FontModel> form)
    {
        var response = await _fontService.Add(new(form.Entity.Font));

        return response.Ok
            ? new(new FontModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response> Save(FormModel<FontModel> form)
    {
        var font = form.Entity.Font;

        return await _fontService.Save(new(font));
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _fontService.Remove(new(new()
        {
            Id = id,
            ContentPortalId = _contentPortalId,
        }));
    }
}

public class FontModel : Observable, IDisposable, INamed
{
    private void HandleFontChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(Font));

    private Font _font;

    public String? Name => Font.Name;

    public FontModel(Font font)
    {
        _font = font;
        _font.PropertyChanged += HandleFontChanged;
    }

    public void Dispose()
    {
        _font.PropertyChanged -= HandleFontChanged;
    }

    public Guid? Id
    {
        get => _font.Id;
        set => _font.Id = value;
    }

    public Font Font
    {
        get => _font;
        set => SetProperty(ref _font, value);
    }
}