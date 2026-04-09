namespace Crudspa.Content.Design.Client.Models;

public abstract class PageEditModelBase : EditModel<Page>, IHandle<PageSaved>, IHandle<PageRemoved>
{
    private void HandleBoxModelChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(BoxModel));

    protected readonly String? Path;
    protected readonly Guid? Id;
    protected readonly INavigator Navigator;
    protected readonly IScrollService ScrollService;

    protected PageEditModelBase(
        String? path,
        Guid? id,
        Boolean isNew,
        IEventBus eventBus,
        INavigator navigator,
        IScrollService scrollService) : base(isNew)
    {
        Path = path;
        Id = id;
        Navigator = navigator;
        ScrollService = scrollService;

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
        if (payload.Id.Equals(Id))
            await Refresh();
    }

    public Task Handle(PageRemoved payload)
    {
        if (payload.Id.Equals(Id))
            Navigator.Close(Path);

        return Task.CompletedTask;
    }

    public List<Orderable> ContentStatusNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public BoxModel? BoxModel
    {
        get;
        set => SetProperty(ref field, value);
    }

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchContentStatusNames());

        await Refresh();
    }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            SetPage(new()
            {
                TypeId = PageTypeIds.StackedSections,
                Title = "New Page",
                StatusId = ContentStatusNames.MinBy(x => x.Ordinal)?.Id,
                ShowNotebook = false,
                ShowGuide = false,
            });
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => FetchPage(Id));

            if (response.Ok)
                SetPage(response.Value);
        }
    }

    public async Task Save()
    {
        if (Entity is null)
            return;

        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => AddPage(Entity));

            if (response.Ok)
            {
                Navigator.GoTo($"{Path.Parent()}/page-{response.Value.Id:D}");
                Navigator.Close(Path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => SavePage(Entity));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    public async Task FetchContentStatusNames()
    {
        var response = await WithAlerts(FetchStatuses, false);

        if (response.Ok)
            ContentStatusNames = response.Value.ToList();
    }

    protected void SetPage(Page page)
    {
        Entity = page;
        Navigator.UpdateTitle(Path, Entity.Title!);

        if (BoxModel is not null)
            BoxModel.PropertyChanged -= HandleBoxModelChanged;

        BoxModel = new(ScrollService, page.Box);
        BoxModel.PropertyChanged += HandleBoxModelChanged;
    }

    protected abstract Task<Response<IList<Orderable>>> FetchStatuses();
    protected abstract Task<Response<Page?>> FetchPage(Guid? id);
    protected abstract Task<Response<Page?>> AddPage(Page page);
    protected abstract Task<Response> SavePage(Page page);
}