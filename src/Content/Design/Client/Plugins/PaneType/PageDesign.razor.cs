using Crudspa.Framework.Core.Shared.Contracts.Ids;

namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class PageDesign : IPaneDesign, IHasPaneId, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);
    private void HandleConfigUpdated(Object? sender, EventArgs args) => ConfigUpdated.InvokeAsync();

    [Parameter] public String? Path { get; set; }
    [Parameter] public String? ConfigJson { get; set; }
    [Parameter] public Guid? PaneId { get; set; }
    [Parameter] public Boolean ReadOnly { get; set; }
    [Parameter] public EventCallback ConfigUpdated { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IPanePageService PanePageService { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;

    public PageDesignModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var config = ConfigJson.FromJson<PageConfig>() ?? new();

        Model = new(config, PaneId, EventBus, ScrollService, PanePageService);
        Model.PropertyChanged += HandleModelChanged;
        Model.ConfigUpdated += HandleConfigUpdated;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.ConfigUpdated -= HandleConfigUpdated;
        Model.Dispose();
    }

    public String? GetConfigJson() => Model.Config.ToJson();

    public Task<Boolean> PrepareForSave() => Model.PrepareForSave();
}

public class PageDesignModel : EditModel<Page>, IHandle<PageSaved>
{
    private void HandleBoxModelChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(BoxModel));

    private readonly IScrollService _scrollService;
    private readonly Guid? _paneId;
    private readonly IPanePageService _panePageService;
    private PageConfig _config;

    public PageDesignModel(PageConfig config, Guid? paneId,
        IEventBus eventBus,
        IScrollService scrollService,
        IPanePageService panePageService) : base(false)
    {
        _config = config;
        _paneId = paneId;
        _scrollService = scrollService;
        _panePageService = panePageService;

        eventBus.Subscribe(this);
    }

    public event EventHandler? ConfigUpdated;

    public override void Dispose()
    {
        if (BoxModel is not null)
        {
            BoxModel.PropertyChanged -= HandleBoxModelChanged;
            BoxModel.Dispose();
        }

        base.Dispose();
    }

    public async Task Handle(PageSaved payload)
    {
        if (payload.Id.Equals(_config.PageId))
            await Refresh();
    }

    public PageConfig Config
    {
        get => _config;
        set => SetProperty(ref _config, value);
    }

    public BoxModel? BoxModel
    {
        get;
        set => SetProperty(ref field, value);
    }

    public async Task Initialize()
    {
        if (Config.PageId.HasNothing())
        {
            var response = await WithWaiting("Adding...", () =>
            {
                var page = new Page
                {
                    TypeId = PageTypeIds.StackedSections,
                    Title = "Pane Page",
                    StatusId = ContentStatusIds.Complete,
                    ShowNotebook = false,
                    ShowGuide = false,
                };

                return _panePageService.AddPage(new(new() { PaneId = _paneId, Page = page }));
            });

            if (response.Ok)
            {
                Config.PageId = response.Value.Id;
                ConfigUpdated?.Invoke(this, EventArgs.Empty);
            }
        }

        await Refresh();
    }

    public async Task Refresh()
    {
        var response = await WithWaiting("Fetching...", () => _panePageService.FetchPage(new(new() { Page = new() { Id = Config.PageId } })));

        if (response.Ok)
            SetPage(response.Value);
    }

    public async Task<Boolean> PrepareForSave()
    {
        if (Entity is null)
            return false;

        if (BoxModel?.Visible == true)
            await BoxModel.Hide();

        var response = await WithWaiting("Saving...", () => _panePageService.SavePage(new(new() { Page = Entity })));
        return response.Ok;
    }

    private void SetPage(Page page)
    {
        Entity = page;

        if (BoxModel is not null)
            BoxModel.PropertyChanged -= HandleBoxModelChanged;

        BoxModel = new(_scrollService, page.Box);
        BoxModel.PropertyChanged += HandleBoxModelChanged;
    }
}