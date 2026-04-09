namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class PageListForModule : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IModuleService ModuleService { get; set; } = null!;

    public PageListForModuleModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Id, ModuleService);
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
        var module = Model.Entity!;

        return ModuleService.AddPage(new(new()
        {
            ModuleId = module.Id,
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

public class PageListForModuleModel(Guid? id, IModuleService moduleService) : EditModel<Module>(false)
{
    public async Task Initialize()
    {
        await Refresh();
    }

    public async Task Refresh()
    {
        ReadOnly = true;

        var response = await WithWaiting("Fetching...", () => moduleService.Fetch(new(new() { Id = id })));

        if (response.Ok)
            Entity = response.Value;
    }
}