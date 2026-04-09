namespace Crudspa.Education.District.Client.Plugins.PaneType;

public partial class SchoolContactRelations : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public ISchoolContactService SchoolContactService { get; set; } = null!;

    public SchoolContactRelationsModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Id, EventBus, SchoolContactService);
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

public class SchoolContactRelationsModel : EditModel<SchoolContact>, IHandle<SchoolContactRelationsSaved>
{
    private readonly Guid? _id;
    private readonly ISchoolContactService _schoolContactService;

    public SchoolContactRelationsModel(Guid? id,
        IEventBus eventBus,
        ISchoolContactService schoolContactService) : base(false)
    {
        _id = id;
        _schoolContactService = schoolContactService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(SchoolContactRelationsSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public async Task Initialize()
    {
        await Refresh();
    }

    public async Task Refresh()
    {
        ReadOnly = true;

        var response = await WithWaiting("Fetching...", () => _schoolContactService.FetchRelations(new(new() { Id = _id })));

        if (response.Ok)
            SetSchoolContact(response.Value);
    }

    public async Task Save()
    {
        var response = await WithWaiting("Saving...", () => _schoolContactService.SaveRelations(new(Entity!)));

        if (response.Ok)
            ReadOnly = true;
    }

    private void SetSchoolContact(SchoolContact schoolContact)
    {
        Entity = schoolContact;
    }
}