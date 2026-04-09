namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class BinderDesign : IPaneDesign, IHasPaneId, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);
    private void HandleConfigUpdated(Object? sender, EventArgs args) => ConfigUpdated.InvokeAsync();

    [Parameter] public String? ConfigJson { get; set; }
    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? PaneId { get; set; }
    [Parameter] public Boolean ReadOnly { get; set; }
    [Parameter] public EventCallback ConfigUpdated { get; set; }

    [Inject] public IPanePageService PanePageService { get; set; } = null!;

    public BinderDesignModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var config = ConfigJson.FromJson<BinderConfig>() ?? new();

        Model = new(config, PaneId, PanePageService);
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

    public Task<Boolean> PrepareForSave() => Task.FromResult(true);

    public String? GetConfigJson() => Model.Config.ToJson();

    public Task<Response<IList<Page>>> FetchPages(Guid? binderId) =>
        PanePageService.FetchPages(new(new() { BinderId = Model.Config.BinderId }));

    public Task<Response<Page?>> AddPage() =>
        PanePageService.AddPage(new(new()
        {
            BinderId = Model.Config.BinderId,
            Page = new()
            {
                TypeId = PageTypeIds.StackedSections,
                Title = "New Page",
                StatusId = Crudspa.Framework.Core.Shared.Contracts.Ids.ContentStatusIds.Draft,
                ShowNotebook = false,
                ShowGuide = false,
            },
        }));

    public Task<Response<Page?>> FetchPage(Guid? pageId) =>
        PanePageService.FetchPage(new(new()
        {
            Page = new() { Id = pageId },
        }));

    public Task<Response> RemovePage(Guid? pageId) =>
        PanePageService.RemovePage(new(new()
        {
            BinderId = Model.Config.BinderId,
            Page = new() { Id = pageId },
        }));

    public Task<Response> SavePageOrder(IList<Page> pages) =>
        PanePageService.SavePageOrder(new(new()
        {
            BinderId = Model.Config.BinderId,
            Pages = pages,
        }));
}

public class BinderDesignModel(
    BinderConfig config,
    Guid? paneId,
    IPanePageService panePageService) : EditModel<Binder>(false)
{
    public event EventHandler? ConfigUpdated;

    public BinderConfig Config
    {
        get;
        set => SetProperty(ref field, value);
    } = config;

    public async Task Initialize()
    {
        if (Config.BinderId.HasNothing())
        {
            var binder = new Binder { TypeId = BinderTypeIds.BackAndNext };
            var response = await WithWaiting("Adding...", () => panePageService.AddBinder(new(new() { PaneId = paneId, Binder = binder })));

            if (response.Ok)
            {
                Config.BinderId = response.Value.Id;
                ConfigUpdated?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}