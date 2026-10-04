namespace Crudspa.Content.Design.Client.Plugins.PaneType;

using Publication = Shared.Contracts.Data.Publication;

public partial class PublicationEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IPublicationService PublicationService { get; set; } = null!;

    public PublicationEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var portalId = Path!.Id("portal");

        Model = new(Path, Id, IsNew, portalId, EventBus, Navigator, PublicationService);
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

public class PublicationEditModel : EditModel<Publication>,
    IHandle<PublicationSaved>, IHandle<PublicationRemoved>, IHandle<PublicationsReordered>
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(args.PropertyName);
    public BatchModel<PublicationResource> PublicationResourcesModel { get; } = new();
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly Guid? _portalId;
    private readonly INavigator _navigator;
    private readonly IPublicationService _publicationService;

    public PublicationEditModel(String? path, Guid? id, Boolean isNew, Guid? portalId,
        IEventBus eventBus,
        INavigator navigator,
        IPublicationService publicationService) : base(isNew)
    {
        _path = path;
        _id = id;
        _portalId = portalId;
        _navigator = navigator;
        _publicationService = publicationService;

        PublicationResourcesModel.PropertyChanged += HandleModelChanged;

        eventBus.Subscribe(this);
    }

    public override void Dispose()
    {
        PublicationResourcesModel.PropertyChanged -= HandleModelChanged;

        base.Dispose();
    }

    public async Task Handle(PublicationSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(PublicationRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public async Task Handle(PublicationsReordered payload)
    {
        if (IsNew || Entity is null)
            return;

        var response = await _publicationService.Fetch(new(new() { Id = _id }));

        if (response.Ok)
        {
            Entity!.Ordinal = response.Value.Ordinal;
            _navigator.UpdateTitle(_path, Entity.Title);
        }
    }





    public List<Orderable> ContentStatusNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Orderable> PublicationGroupNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<Orderable> PublicationResourceTypeNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchContentStatusNames(),
            FetchPublicationGroupNames(),
            FetchPublicationResourceTypeNames());

        await Refresh();
    }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            var publication = new Publication
            {
                PortalId = _portalId,
                StatusId = Crudspa.Framework.Core.Shared.Contracts.Ids.ContentStatusIds.Draft,
                Title = "New Publication",
            };

            SetPublication(publication);
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _publicationService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
                SetPublication(response.Value);
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _publicationService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/publication-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _publicationService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    public void AddPublicationResource()
    {
        PublicationResourcesModel.Entities.Add(new()
        {
            Id = Guid.NewGuid(),
            PublicationId = _id,
            Ordinal = PublicationResourcesModel.Entities.Count,
        });
    }

    public async Task FetchContentStatusNames()
    {
        var response = await WithAlerts(() => _publicationService.FetchContentStatusNames(new()), false);
        if (response.Ok) ContentStatusNames = response.Value.ToList();
    }

    public async Task FetchPublicationGroupNames()
    {
        var response = await WithAlerts(() => _publicationService.FetchPublicationGroupNames(new(new() { Id = _portalId })), false);
        if (response.Ok) PublicationGroupNames = response.Value.ToList();
    }

    public async Task FetchPublicationResourceTypeNames()
    {
        var response = await WithAlerts(() => _publicationService.FetchPublicationResourceTypeNames(new()), false);
        if (response.Ok) PublicationResourceTypeNames = response.Value.ToObservable();
    }

    private void SetPublication(Publication publication)
    {
        Entity = publication;
        PublicationResourcesModel.Entities = publication.PublicationResources;
        _navigator.UpdateTitle(_path, Entity.Title);
    }
}