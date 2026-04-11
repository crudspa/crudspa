namespace Crudspa.Content.Design.Client.Models;

public class SectionListModel : Observable, IDisposable, INamed, IOrderable
{
    private void HandleSectionChanged(Object? sender, PropertyChangedEventArgs args)
    {
        RaisePropertyChanged(nameof(Section));
        RaisePropertyChanged(nameof(Name));
    }

    private Section _section;

    public SectionListModel(Section section)
    {
        _section = section;
        _section.PropertyChanged += HandleSectionChanged;
    }

    public void Dispose()
    {
        _section.PropertyChanged -= HandleSectionChanged;
    }

    public String? Name => String.Empty;

    public Guid? Id
    {
        get => _section.Id;
        set => _section.Id = value;
    }

    public Int32? Ordinal
    {
        get => _section.Ordinal;
        set => _section.Ordinal = value;
    }

    public Section Section
    {
        get => _section;
        set => SetProperty(ref _section, value);
    }
}

public abstract class SectionEditModel : EditModel<Section>, IHandle<SectionSaved>, IHandle<SectionRemoved>, IHandle<SectionsReordered>
{
    private String BuildTitle(Section? section)
    {
        if (IsNew)
            return "New Section";

        return section?.Ordinal is not null
            ? $"Section {section.Ordinal.Value + 1}"
            : "Section";
    }

    protected readonly String? Path;
    protected readonly Guid? Id;
    private readonly Guid? _defaultPageId;
    private readonly INavigator _navigator;
    private readonly IScrollService _scrollService;
    private readonly ISectionService _sectionService;
    private readonly IItemService _itemService;
    private readonly IContainerService _containerService;

    protected SectionEditModel(
        String? path,
        Guid? id,
        Boolean isNew,
        Guid? defaultPageId,
        IEventBus eventBus,
        INavigator navigator,
        IScrollService scrollService,
        ISectionService sectionService,
        IItemService itemService,
        IContainerService containerService)
        : base(isNew)
    {
        Path = path;
        Id = id;
        _defaultPageId = defaultPageId;
        _navigator = navigator;
        _scrollService = scrollService;
        _sectionService = sectionService;
        _itemService = itemService;
        _containerService = containerService;

        AddElementModel = new(scrollService, null);
        SectionShapeModel = new(scrollService, this);
        ElementEditBatchModel = new();
        AutoShowSectionShape = isNew;
        WaitingOn = "Initializing...";
        Waiting = true;

        eventBus.Subscribe(this);
    }

    public override void Dispose()
    {
        DisposeEditorModels();
        AddElementModel.Dispose();
        SectionShapeModel.Dispose();
        base.Dispose();
    }

    public async Task Handle(SectionSaved payload)
    {
        if (payload.Id.Equals(Id))
            await Refresh();
    }

    public Task Handle(SectionRemoved payload)
    {
        if (payload.Id.Equals(Id))
            _navigator.Close(Path);

        return Task.CompletedTask;
    }

    public async Task Handle(SectionsReordered payload)
    {
        if (IsNew || Entity?.PageId is null || !payload.PageId.Equals(Entity.PageId))
            return;

        var response = await FetchSection(Id);

        if (response.Ok && response.Value is not null && Entity is not null)
        {
            Entity.Ordinal = response.Value.Ordinal;
            _navigator.UpdateTitle(Path, BuildTitle(Entity));
        }
    }

    public List<ElementType> ElementTypes
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public BatchModel<ElementEditModel> ElementEditBatchModel
    {
        get;
        private set => SetProperty(ref field, value);
    }

    public Guid? PendingAddedElementId
    {
        get;
        private set => SetProperty(ref field, value);
    }

    public AddElementModel AddElementModel { get; }
    public SectionShapeModel SectionShapeModel { get; }

    protected Guid? DefaultPageId => _defaultPageId;

    public Boolean AutoShowSectionShape
    {
        get;
        private set => SetProperty(ref field, value);
    }

    public BoxModel? BoxModel
    {
        get;
        private set => SetProperty(ref field, value);
    }

    public ContainerModel? ContainerModel
    {
        get;
        private set => SetProperty(ref field, value);
    }

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchElementTypes());

        await Refresh();
    }

    public async Task Refresh()
    {
        await AddElementModel.Hide();

        if (IsNew)
        {
            ReadOnly = false;

            await WithWaiting("Initializing...", async () =>
            {
                var section = new Section
                {
                    Id = Id ?? Guid.NewGuid(),
                    PageId = _defaultPageId,
                    TypeId = SectionTypeIds.Responsive,
                    Container = new()
                    {
                        DirectionId = DirectionIds.Row,
                        WrapId = WrapIds.Wrap,
                        JustifyContentId = JustifyContentIds.Center,
                        AlignItemsId = AlignItemsIds.Center,
                        AlignContentId = AlignContentIds.Start,
                    },
                    Ordinal = 0,
                };

                await SetSection(section);
                return new Response<Section?>(section);
            });
        }
        else
        {
            ReadOnly = true;

            await WithWaiting("Fetching...", async () =>
            {
                var response = await FetchSection(Id);

                if (response.Ok && response.Value is not null)
                    await SetSection(response.Value);

                return response;
            });
        }
    }

    public async Task Save()
    {
        if (Entity is null)
            return;

        PrepareSectionForSave();

        if (!IsValid(Entity))
            return;

        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => AddSection(Entity));

            if (response.Ok && response.Value is not null)
            {
                _navigator.GoTo($"{Path.Parent()}/section-{response.Value.Id:D}");
                _navigator.Close(Path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => SaveSection(Entity));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    public async Task AddElement()
    {
        if (Entity is null || !AddElementModel.TypeId.HasValue)
            return;

        var request = new Request<ElementSpec>(new()
        {
            ElementType = ElementTypes.First(x => x.Id.Equals(AddElementModel.TypeId)),
            SectionId = Entity.Id,
            Ordinal = ElementEditBatchModel.Entities.Count,
        });

        var response = await AddElementModel.WithWaiting("Creating...", () => _sectionService.CreateElement(request));

        if (response.Ok && response.Value?.Id is Guid elementId)
        {
            var elementModel = new ElementEditModel(response.Value, _scrollService, _itemService, ElementTypes);
            await elementModel.Initialize();
            ElementEditBatchModel.AddEntity(elementModel);
            await AddElementModel.Hide();
            PendingAddedElementId = elementId;
        }
    }

    public async Task AddElement(Guid? typeId)
    {
        AddElementModel.TypeId = typeId;
        await AddElement();
    }

    public async Task<Response<SectionElement?>> CreateShapeElement(Guid? elementTypeId, Int32 ordinal)
    {
        if (Entity is null)
            return new("The section is not ready yet.");

        var elementType = ElementTypes.FirstOrDefault(x => x.Id.Equals(elementTypeId));

        if (elementType is null)
            return new($"Element type '{elementTypeId}' was not found.");

        return await _sectionService.CreateElement(new(new()
        {
            ElementType = elementType,
            SectionId = Entity.Id,
            Ordinal = ordinal,
        }));
    }

    public async Task ReplaceElements(IEnumerable<SectionElement> elements)
    {
        if (Entity is null)
            return;

        foreach (var elementModel in ElementEditBatchModel.Entities)
            elementModel.Dispose();

        ElementEditBatchModel.Dispose();

        var nextElements = elements.ToList();
        nextElements.EnsureOrder();

        Entity.Elements = nextElements.ToObservable();
        var elementModels = nextElements
            .Select(x => new ElementEditModel(x, _scrollService, _itemService, ElementTypes))
            .ToList();
        await Task.WhenAll(elementModels.Select(x => x.Initialize()));

        ElementEditBatchModel = new() { Entities = elementModels.ToObservable() };
    }

    public Boolean ConsumeAutoShowSectionShape()
    {
        if (!AutoShowSectionShape)
            return false;

        AutoShowSectionShape = false;
        return true;
    }

    public Guid? ConsumePendingAddedElementId()
    {
        var id = PendingAddedElementId;
        PendingAddedElementId = null;
        return id;
    }

    public async Task FetchElementTypes()
    {
        var response = await WithAlerts(() => _sectionService.FetchElementTypes(new()), false);

        if (response.Ok)
        {
            ElementTypes = response.Value.ToList();
            AddElementModel.TypeId ??= ElementTypes.OrderBy(x => x.Ordinal).FirstOrDefault()?.Id;
        }
    }

    private async Task SetSection(Section section)
    {
        DisposeEditorModels();

        Entity = section;
        _navigator.UpdateTitle(Path, BuildTitle(section));

        BoxModel = new(_scrollService, section.Box);

        ContainerModel = new(_scrollService, _containerService, section.Container);
        await ContainerModel.Initialize();

        var elementModels = section.Elements
            .Select(x => new ElementEditModel(x, _scrollService, _itemService, ElementTypes))
            .ToList();
        await Task.WhenAll(elementModels.Select(x => x.Initialize()));

        ElementEditBatchModel = new() { Entities = elementModels.ToObservable() };

        if (!ElementTypes.Any(x => x.Id.Equals(AddElementModel.TypeId)))
            AddElementModel.TypeId = ElementTypes.OrderBy(x => x.Ordinal).FirstOrDefault()?.Id;
    }

    private void PrepareSectionForSave()
    {
        Entity!.Elements.Clear();

        foreach (var elementModel in ElementEditBatchModel.Entities)
        {
            ((IElementDesign)elementModel.DesignComponent.Instance!).PrepareForSave();
            Entity.Elements.Add(elementModel.Element);
        }
    }

    private void DisposeEditorModels()
    {
        foreach (var elementModel in ElementEditBatchModel.Entities)
            elementModel.Dispose();

        ElementEditBatchModel.Dispose();
        BoxModel?.Dispose();
        ContainerModel?.Dispose();
    }

    protected abstract Task<Response<Section?>> FetchSection(Guid? id);
    protected abstract Task<Response<Section?>> AddSection(Section section);
    protected abstract Task<Response> SaveSection(Section section);
}

public class AddElementModel(IScrollService scrollService, Guid? typeId) : ModalModel(scrollService)
{
    public Guid? TypeId { get; set; } = typeId;
}