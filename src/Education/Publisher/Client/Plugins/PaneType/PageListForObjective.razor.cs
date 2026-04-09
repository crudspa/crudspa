namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class PageListForObjective : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IObjectiveService ObjectiveService { get; set; } = null!;

    public PageListForObjectiveModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Id, ObjectiveService);
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
        var objective = Model.Entity!;

        return ObjectiveService.AddPage(new(new()
        {
            ObjectiveId = objective.Id,
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

public class PageListForObjectiveModel(Guid? id, IObjectiveService objectiveService) : EditModel<Objective>(false)
{
    public async Task Initialize()
    {
        await Refresh();
    }

    public async Task Refresh()
    {
        ReadOnly = true;

        var response = await WithWaiting("Fetching...", () => objectiveService.Fetch(new(new() { Id = id })));

        if (response.Ok)
            Entity = response.Value;
    }
}