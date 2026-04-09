namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class PageListForTrifold : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public ITrifoldService TrifoldService { get; set; } = null!;

    public PageListForTrifoldModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Id, TrifoldService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }

    public Task<Response<Page?>> AddPage()
    {
        var trifold = Model.Entity!;

        return TrifoldService.AddPage(new(new()
        {
            TrifoldId = trifold.Id,
            Page = new()
            {
                TypeId = PageTypeIds.StackedSections,
                Title = "New Page",
                StatusId = Crudspa.Framework.Core.Shared.Contracts.Ids.ContentStatusIds.Draft,
                ShowNotebook = false,
                ShowGuide = false,
            },
        }));
    }
}

public class PageListForTrifoldModel(Guid? id, ITrifoldService trifoldService) : EditModel<Trifold>(false)
{
    public async Task Initialize()
    {
        await Refresh();
    }

    public async Task Refresh()
    {
        ReadOnly = true;

        var response = await WithWaiting("Fetching...", () => trifoldService.Fetch(new(new() { Id = id })));

        if (response.Ok)
            Entity = response.Value;
    }
}