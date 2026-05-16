namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class BinderDesign : IPaneDesign, IHasPaneId, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? ConfigJson { get; set; }
    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? PaneId { get; set; }
    [Parameter] public Boolean ReadOnly { get; set; }
    [Parameter] public EventCallback ConfigUpdated { get; set; }

    [Inject] public IPanePageService PanePageService { get; set; } = null!;

    public BinderDesignModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(PaneId, PanePageService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }

    public Task<Boolean> PrepareForSave() => Task.FromResult(true);

    public String? GetConfigJson() => null;

    public Task<Response<IList<Page>>> FetchPages(Guid? binderId) =>
        PanePageService.FetchPages(new(new() { BinderId = Model.BinderId }));

    public Task<Response<Page?>> AddPage() =>
        PanePageService.AddPage(new(new()
        {
            BinderId = Model.BinderId,
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
            BinderId = Model.BinderId,
            Page = new() { Id = pageId },
        }));

    public Task<Response> SavePageOrder(IList<Page> pages) =>
        PanePageService.SavePageOrder(new(new()
        {
            BinderId = Model.BinderId,
            Pages = pages,
        }));
}

public class BinderDesignModel(
    Guid? paneId,
    IPanePageService panePageService) : EditModel<Binder>(false)
{
    public Guid? BinderId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public async Task Initialize()
    {
        if (BinderId.HasNothing())
        {
            var binderPaneResponse = await WithWaiting("Fetching...", () =>
                panePageService.FetchBinderPane(new(new() { PaneId = paneId })));

            if (binderPaneResponse.Ok)
                BinderId = binderPaneResponse.Value?.BinderId;
        }

        if (BinderId.HasNothing())
        {
            var binder = new Binder { TypeId = BinderTypeIds.BackAndNext };
            var response = await WithWaiting("Adding...", () => panePageService.AddBinder(new(new() { PaneId = paneId, Binder = binder })));

            if (response.Ok)
                BinderId = response.Value.Id;
        }
    }
}