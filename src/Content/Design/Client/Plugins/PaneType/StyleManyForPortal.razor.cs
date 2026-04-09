using Crudspa.Content.Display.Client.Plugins;

namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class StyleManyForPortal : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IStyleService StyleService { get; set; } = null!;

    public StyleManyForPortalModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, StyleService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class StyleManyForPortalModel : ManyModel<StyleModel>,
    IHandle<StyleSaved>,
    IHandle<FontSaved>, IHandle<FontRemoved>
{
    private readonly IStyleService _styleService;
    private readonly Guid? _contentPortalId;

    public StyleManyForPortalModel(IEventBus eventBus, IScrollService scrollService, IStyleService styleService, Guid? contentPortalId)
        : base(scrollService)
    {
        _styleService = styleService;
        _contentPortalId = contentPortalId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(StyleSaved payload) => await Replace(payload.Id, payload.ContentPortalId);
    public async Task Handle(FontSaved payload) => await Refresh();
    public async Task Handle(FontRemoved payload) => await Refresh();

    public async Task Initialize()
    {
        await Refresh();
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request<ContentPortal>(new() { Id = _contentPortalId });
        var response = await WithWaiting("Fetching...", () => _styleService.FetchForContentPortal(request), resetAlerts);

        if (response.Ok)
            SetForms(response.Value.Select(x => new StyleModel(x)));
    }

    public override Task Create()
    {
        return Task.CompletedTask;
    }

    public override Boolean InScope(Guid? scopeId)
    {
        return scopeId is null || scopeId.Equals(_contentPortalId);
    }

    public override async Task<Response<StyleModel?>> Fetch(Guid? id)
    {
        var response = await _styleService.Fetch(new(new() { Id = id }));

        return response.Ok
            ? new(new StyleModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override Task<Response<StyleModel?>> Add(FormModel<StyleModel> form)
    {
        return Task.FromResult(new Response<StyleModel?>("Styles are provisioned automatically and cannot be added."));
    }

    public override async Task<Response> Save(FormModel<StyleModel> form)
    {
        var style = form.Entity.Style;

        var builder = (IStyleDesign)form.Entity.StyleDesign.Instance!;

        var errors = builder.Validate();

        if (errors.HasItems())
            return new() { Errors = errors };

        style.ConfigJson = builder.GetConfigJson();

        return await _styleService.Save(new(style));
    }

    public override Task<Response> Remove(Guid? id)
    {
        return Task.FromResult(new Response("Styles are provisioned automatically and cannot be removed."));
    }
}

public class StyleModel : Observable, IDisposable, INamed
{
    private void HandleStyleChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(Style));

    private Style _style;

    public String? Name => Style.Rule.Name;

    public StyleModel(Style style)
    {
        _style = style;
        _style.PropertyChanged += HandleStyleChanged;
    }

    public void Dispose()
    {
        _style.PropertyChanged -= HandleStyleChanged;
    }

    public Guid? Id
    {
        get => _style.Id;
        set => _style.Id = value;
    }

    public Style Style
    {
        get => _style;
        set => SetProperty(ref _style, value);
    }

    public StyleDisplayPlugin RuleDisplay { get; set; } = null!;
    public StyleDesignPlugin StyleDesign { get; set; } = null!;
}