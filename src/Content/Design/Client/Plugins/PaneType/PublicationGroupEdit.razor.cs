
namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class PublicationGroupEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IPublicationGroupService PublicationGroupService { get; set; } = null!;

    public PublicationGroupEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var portalId = Path!.Id("portal");

        Model = new(Path, Id, IsNew, portalId, EventBus, Navigator, PublicationGroupService);
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
        if (Model.IsNew)
            Navigator.Close(Path);
        else
            await Model.Refresh();
    }
}

public class PublicationGroupEditModel : EditModel<PublicationGroup>,
    IHandle<PublicationGroupSaved>, IHandle<PublicationGroupRemoved>, IHandle<PublicationGroupsReordered>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly Guid? _portalId;
    private readonly INavigator _navigator;
    private readonly IPublicationGroupService _publicationGroupService;

    public PublicationGroupEditModel(String? path, Guid? id, Boolean isNew, Guid? portalId,
        IEventBus eventBus,
        INavigator navigator,
        IPublicationGroupService publicationGroupService) : base(isNew)
    {
        _path = path;
        _id = id;
        _portalId = portalId;
        _navigator = navigator;
        _publicationGroupService = publicationGroupService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(PublicationGroupSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(PublicationGroupRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public async Task Handle(PublicationGroupsReordered payload)
    {
        if (IsNew || Entity is null)
            return;

        var response = await _publicationGroupService.Fetch(new(new() { Id = _id }));

        if (response.Ok)
        {
            Entity!.Ordinal = response.Value.Ordinal;
            _navigator.UpdateTitle(_path, Entity.Name);
        }
    }

    public async Task Initialize()
    {
        await Refresh();
    }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            var publicationGroup = new PublicationGroup
            {
                PortalId = _portalId,
                Name = "New Publication Group",
            };

            SetPublicationGroup(publicationGroup);
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _publicationGroupService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
                SetPublicationGroup(response.Value);
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _publicationGroupService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/publication-group-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _publicationGroupService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }


    private void SetPublicationGroup(PublicationGroup publicationGroup)
    {
        Entity = publicationGroup;
        _navigator.UpdateTitle(_path, Entity.Name);
    }
}