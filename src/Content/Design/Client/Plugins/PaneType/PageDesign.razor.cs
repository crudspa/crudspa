using Crudspa.Framework.Core.Shared.Contracts.Ids;

namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class PageDesign : IPaneDesign, IHasPaneId, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

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
        Model = new(PaneId, EventBus, ScrollService, PanePageService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }

    public String? GetConfigJson() => null;

    public async Task<Boolean> PrepareForSave()
    {
        var saved = await Model.PrepareForSave();
        return saved;
    }
}

public class PageDesignModel : EditModel<Page>, IHandle<PageSaved>
{
    private void HandleBoxModelChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(BoxModel));

    private readonly IScrollService _scrollService;
    private readonly Guid? _paneId;
    private readonly IPanePageService _panePageService;

    public PageDesignModel(Guid? paneId,
        IEventBus eventBus,
        IScrollService scrollService,
        IPanePageService panePageService) : base(false)
    {
        _paneId = paneId;
        _scrollService = scrollService;
        _panePageService = panePageService;

        eventBus.Subscribe(this);
    }

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
        if (payload.Id.Equals(PageId))
            await Refresh();
    }

    public Guid? PageId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public BoxModel? BoxModel
    {
        get;
        set => SetProperty(ref field, value);
    }

    public async Task Initialize()
    {
        if (PageId.HasNothing())
        {
            var pagePaneResponse = await WithWaiting("Fetching...", () =>
                _panePageService.FetchPagePane(new(new() { PaneId = _paneId })));

            if (pagePaneResponse.Ok)
                PageId = pagePaneResponse.Value?.PageId;
        }

        if (PageId.HasNothing())
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
                PageId = response.Value.Id;
        }

        await Refresh();
    }

    public async Task Refresh()
    {
        var response = await WithWaiting("Fetching...", () => _panePageService.FetchPage(new(new() { Page = new() { Id = PageId } })));

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