namespace Crudspa.Education.School.Client.Plugins.PaneType;

using School = Shared.Contracts.Data.School;

public partial class SchoolEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public ISchoolService SchoolService { get; set; } = null!;

    public SchoolEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, SchoolService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }

    private async Task HandleCancelClicked()
    {
        await Model.Refresh();
    }
}

public class SchoolEditModel : EditModel<School>,
    IHandle<SchoolSaved>
{
    private readonly ISchoolService _schoolService;

    public List<Named> PermissionNames = [];

    public SchoolEditModel(IEventBus eventBus,
        ISchoolService schoolService) : base(false)
    {
        _schoolService = schoolService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(SchoolSaved payload)
    {
        if (payload.Id.Equals(Entity?.Id))
            await Refresh();
    }

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchPermissionNames());

        await Refresh();
    }

    public async Task Refresh()
    {
        ReadOnly = true;

        var response = await WithWaiting("Fetching...", () => _schoolService.Fetch(new()));

        if (response.Ok)
            SetSchool(response.Value);
    }

    public async Task Save()
    {
        var response = await WithWaiting("Saving...", () => _schoolService.Save(new(Entity!)));

        if (response.Ok)
            ReadOnly = true;
    }

    private async Task FetchPermissionNames()
    {
        var response = await WithAlerts(() => _schoolService.FetchPermissionNames(new()), false);
        if (response.Ok) PermissionNames = response.Value.ToList();
    }

    private void SetSchool(School school)
    {
        Entity = school;
    }
}