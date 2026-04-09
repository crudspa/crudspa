using Crudspa.Content.Display.Client.Components;

namespace Crudspa.Content.Display.Client.Plugins.PaneType;

public partial class PageDisplay : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }
    [Parameter] public ImageFile? GuideImage { get; set; }
    [Parameter] public Guid? PortalId { get; set; }

    [Inject] public IPageRunService PageRunService { get; set; } = null!;
    [Inject] public IEventBus EventBus { get; set; } = null!;

    public PageDisplayModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var config = ConfigJson.FromJson<PageConfig>();

        if (config is not null && config.PageId.HasSomething())
            Id = config.PageId;

        Model = new(Id, PageRunService, GuideImage, EventBus);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class PageDisplayModel : ScreenModel, IHandle<PageContentChanged>
{
    private readonly Guid? _id;
    private readonly IPageRunService _pageRunService;
    private readonly ImageFile? _guideImage;

    public PageDisplayModel(Guid? id,
        IPageRunService pageRunService,
        ImageFile? guideImage,
        IEventBus eventBus)
    {
        _id = id;
        _pageRunService = pageRunService;
        _guideImage = guideImage;

        eventBus.Subscribe(this);
    }

    public GuideModel? GuideModel
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Page? Page
    {
        get;
        set => SetProperty(ref field, value);
    }

    public async Task Handle(PageContentChanged payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public async Task Refresh()
    {
        var response = await WithWaiting("Fetching...", () => _pageRunService.Fetch(new(new() { Id = _id })));

        if (response.Ok)
            SetPage(response.Value);
    }

    private void SetPage(Page page)
    {
        page.Sections ??= [];
        page.Sections = page.Sections.OrderBy(x => x.Ordinal).ToObservable();

        Page = null;
        Page = page;

        GuideModel = new()
        {
            Image = _guideImage,
            Text = page.GuideText,
            Audio = page.GuideAudio,
        };
    }
}