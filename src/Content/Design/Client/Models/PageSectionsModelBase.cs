namespace Crudspa.Content.Design.Client.Models;

public abstract class PageSectionsModelBase : ListOrderablesModel<SectionListModel>,
    IHandle<SectionAdded>, IHandle<SectionSaved>, IHandle<SectionRemoved>, IHandle<SectionsReordered>
{
    protected PageSectionsModelBase(IEventBus eventBus, IScrollService scrollService, Guid? pageId)
        : base(scrollService)
    {
        PageId = pageId;

        eventBus.Subscribe(this);
    }

    protected Guid? PageId { get; private set; }

    public async Task Initialize() => await Refresh();

    public async Task Handle(SectionAdded payload) => await Replace(payload.Id, payload.PageId);
    public async Task Handle(SectionSaved payload) => await Replace(payload.Id, payload.PageId);
    public async Task Handle(SectionRemoved payload) => await Rid(payload.Id, payload.PageId);
    public async Task Handle(SectionsReordered payload) => await Refresh();

    public override Boolean InScope(Guid? scopeId)
    {
        return scopeId is null || (PageId is not null && scopeId.Equals(PageId));
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var response = await WithWaiting("Fetching...", () => FetchSections(PageId), resetAlerts);

        if (response.Ok)
        {
            var sections = response.Value.ToList();
            PageId ??= sections.FirstOrDefault()?.PageId;
            SetCards(sections.Select(x => new SectionListModel(x)).ToList());
        }
    }

    public override async Task<Response<SectionListModel?>> Fetch(Guid? id)
    {
        var response = await FetchSection(new() { Id = id, PageId = PageId });

        if (!response.Ok || response.Value is null)
            return new() { Errors = response.Errors };

        PageId ??= response.Value.PageId;
        return new(new SectionListModel(response.Value));
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await RemoveSection(new()
        {
            Id = id,
            PageId = PageId,
        });
    }

    public override async Task<Response> SaveOrder()
    {
        var sections = Cards.Select(x => x.Entity.Section).ToList();
        return await WithWaiting("Saving...", () => SaveSectionOrder(sections));
    }

    protected abstract Task<Response<IList<Section>>> FetchSections(Guid? pageId);
    protected abstract Task<Response<Section?>> FetchSection(Section section);
    protected abstract Task<Response> RemoveSection(Section section);
    protected abstract Task<Response> SaveSectionOrder(IList<Section> sections);
}