using Crudspa.Content.Display.Client.Components;

namespace Crudspa.Content.Display.Client.Plugins.BinderType;

public partial class BackAndNextDisplay : IBinderDisplay, IDisposable, IHandle<ValidateBinder>
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName == nameof(BackAndNextDisplayModel.Page))
            ElementDisplayModelsBySection.Clear();

        InvokeAsync(StateHasChanged);
    }

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }
    [Parameter] public EventCallback BinderCompleted { get; set; }
    [Parameter] public ImageFile? GuideImage { get; set; }
    [Parameter] public Boolean Shadowed { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IBinderRunService BinderRunService { get; set; } = null!;
    [Inject] public IElementProgressService ElementProgressService { get; set; } = null!;
    [Inject] public IPageRunService PageRunService { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;

    public BackAndNextDisplayModel Model { get; set; } = null!;
    public Dictionary<Section, IReadOnlyList<ElementDisplayModel>> ElementDisplayModelsBySection { get; } = [];

    protected override async Task OnInitializedAsync()
    {
        var config = ConfigJson.FromJson<BinderConfig>();

        if (config is not null && config.BinderId.HasSomething())
            Id = config.BinderId;

        Model = new(Id, BinderRunService, ElementProgressService, PageRunService, ScrollService, GuideImage, EventBus);
        Model.PropertyChanged += HandleModelChanged;

        EventBus.Subscribe(this);

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }

    private Task HandleSectionDisplayModelsChanged(Section section, IReadOnlyList<ElementDisplayModel> elementDisplayModels)
    {
        ElementDisplayModelsBySection[section] = elementDisplayModels;
        return Task.CompletedTask;
    }

    public async Task Back()
    {
        if (MoveBack())
            await Model.LoadPage();
    }

    public async Task Next()
    {
        if (MoveNext())
            await Model.LoadPage();
    }

    public async Task Finish()
    {
        if (CheckMoveNext())
        {
            var showAlert = false;

            await Model.WithWaiting("Saving progress...", async () =>
            {
                if (BinderCompleted.HasDelegate)
                    await BinderCompleted.InvokeAsync();
                else
                {
                    await BinderRunService.AddCompleted(new(new() { BinderId = Id, DeviceTimestamp = DateTimeOffset.Now }));
                    showAlert = true;
                }

                return new();
            });

            MoveToFirst();

            await Model.LoadPage(finished: showAlert);
        }
    }

    private Boolean MoveBack()
    {
        if (Model.PageNumber > 1)
        {
            Model.PageNumber--;
            UpdatePagingState();
            return true;
        }

        return false;
    }

    private Boolean MoveNext()
    {
        if (CheckMoveNext())
        {
            Model.PageNumber++;
            UpdatePagingState();
            return true;
        }

        return false;
    }

    private void MoveToFirst()
    {
        Model.PageNumber = 1;
        UpdatePagingState();
    }

    private void UpdatePagingState()
    {
        Model.CanMoveBack = Model.PageNumber > 1;
        Model.CanMoveNext = Model.PageNumber < Model.PageCount;
        Model.IsValidating = false;
        Model.UpdatePositionText();
    }

    private Boolean CheckMoveNext()
    {
        return Validate() == 0;
    }

    public Task Handle(ValidateBinder payload)
    {
        if (Model.IsValidating)
            Validate();

        return Task.CompletedTask;
    }

    public Int32 Validate()
    {
        var count = ElementDisplayModelsBySection.Values.SelectMany(x => x
                .Where(x => x.Element.ElementType!.SupportsInteraction == true && x.Element.RequireInteraction == true))
            .Count(elementModel => elementModel.CompletionStatus != ElementDisplayModel.CompletionStatuses.Complete);

        Model.ValidationText = count switch
        {
            1 => "1 activity incomplete",
            > 1 => $"{count} activities incomplete",
            _ => String.Empty,
        };

        Model.IsValidating = count > 0;

        return count;
    }
}

public class BackAndNextDisplayModel : ScreenModel, IHandle<PageContentChanged>
{
    private Page? _page;
    private readonly Guid? _id;
    private readonly IBinderRunService _binderRunService;
    private readonly IElementProgressService _elementProgressService;
    private readonly IPageRunService _pageRunService;
    private readonly IScrollService _scrollService;
    private readonly ImageFile? _guideImage;
    private readonly Dictionary<Guid, Page> _pageCache = [];
    private readonly HashSet<Guid> _prefetchingPageIds = [];
    private readonly HashSet<Guid> _viewedPageIds = [];
    private Task<Response<IList<ElementProgress>>>? _elementProgressFetchTask;

    public BackAndNextDisplayModel(Guid? id,
        IBinderRunService binderRunService,
        IElementProgressService elementProgressService,
        IPageRunService pageRunService,
        IScrollService scrollService,
        ImageFile? guideImage,
        IEventBus eventBus)
    {
        _id = id;
        _binderRunService = binderRunService;
        _elementProgressService = elementProgressService;
        _pageRunService = pageRunService;
        _scrollService = scrollService;
        _guideImage = guideImage;

        eventBus.Subscribe(this);
    }

    public Binder? Binder
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Page? Page
    {
        get => _page;
        set => SetProperty(ref _page, value);
    }

    public Boolean CanMoveBack
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean CanMoveNext
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32 PageCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32 PageNumber
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String PositionText
    {
        get;
        set => SetProperty(ref field, value);
    } = "0 of 0";

    public String ValidationText
    {
        get;
        set => SetProperty(ref field, value);
    } = String.Empty;

    public Boolean IsValidating
    {
        get;
        set => SetProperty(ref field, value);
    }

    public GuideModel? GuideModel
    {
        get;
        set => SetProperty(ref field, value);
    }

    public async Task Handle(PageContentChanged payload)
    {
        if (payload.Id.HasValue)
            _pageCache.Remove(payload.Id.Value);

        if (_page is not null && payload.Id.Equals(_page.Id))
            await LoadPage();
    }

    public async Task Refresh()
    {
        _elementProgressFetchTask = _elementProgressService.FetchAll(new());
        var response = await WithWaiting("Fetching...", () => _binderRunService.FetchBinder(new(new() { Id = _id })));

        if (response.Ok)
            await SetBinder(response.Value);
    }

    private async Task SetBinder(Binder binder)
    {
        _pageCache.Clear();
        _prefetchingPageIds.Clear();
        _viewedPageIds.Clear();

        binder.Pages ??= [];
        binder.Pages = binder.Pages.OrderBy(x => x.Ordinal).ToObservable();

        if (binder.InitialPage?.Id is Guid initialPageId)
            CachePage(initialPageId, binder.InitialPage);

        Binder = binder;

        if (binder.Pages!.IsEmpty())
        {
            PageCount = 0;
            PageNumber = 0;
            CanMoveBack = false;
            CanMoveNext = false;
        }
        else
        {
            PageCount = binder.Pages!.Count;

            if (binder.LastPageViewed.HasValue)
            {
                var page = binder.Pages.FirstOrDefault(x => x.Id.Equals(binder.LastPageViewed));
                PageNumber = page is null ? 1 : binder.Pages.IndexOf(page) + 1;
            }
            else
                PageNumber = 1;

            CanMoveBack = PageNumber > 1;
            CanMoveNext = PageNumber < PageCount;
        }

        UpdatePositionText();

        await LoadPage(first: true);
    }

    public void UpdatePositionText()
    {
        PositionText = $"Page {PageNumber} of {PageCount}";
    }

    public async Task LoadPage(Boolean first = false, Boolean finished = false)
    {
        if (Binder?.Pages is null || Binder.Pages.Count == 0)
        {
            Page = null;
            return;
        }

        var pageId = Binder.Pages[PageNumber - 1].Id;
        if (!pageId.HasValue)
            return;

        var elementProgressTask = _elementProgressFetchTask ?? _elementProgressService.FetchAll(new());
        var page = await GetPage(pageId.Value);
        if (page is null)
            return;

        await elementProgressTask;
        await ShowPage(page, first, finished);
        _ = PrefetchAdjacentPages();
    }

    private async Task<Page?> GetPage(Guid pageId)
    {
        if (_pageCache.TryGetValue(pageId, out var cachedPage))
            return cachedPage.DeepClone();

        var fetchedPage = await FetchPage(pageId, true);

        if (fetchedPage is null)
            return null;

        CachePage(pageId, fetchedPage);

        return fetchedPage.DeepClone();
    }

    private async Task<Page?> FetchPage(Guid pageId, Boolean showWaiting)
    {
        var response = showWaiting
            ? await WithWaiting("Fetching...", () => _binderRunService.FetchPage(new(new() { Id = pageId })))
            : await _binderRunService.FetchPage(new(new() { Id = pageId }));

        return response.Ok ? response.Value : null;
    }

    private void CachePage(Guid requestedPageId, Page page)
    {
        var cacheKey = page.Id.GetValueOrDefault(requestedPageId);
        _pageCache[cacheKey] = page;
    }

    private async Task ShowPage(Page page, Boolean first, Boolean finished)
    {
        page.Sections ??= [];
        page.Sections = page.Sections.OrderBy(x => x.Ordinal).ToObservable();

        Page = null;
        Page = page;

        GuideModel = new()
        {
            Image = _guideImage,
            Text = Page.GuideText,
            Audio = Page.GuideAudioFile,
        };

        if (!first)
            await _scrollService.ToId(_id.GetValueOrDefault());

        if (finished)
            Alerts.Add(new() { Type = Alert.AlertType.Success, Message = "You completed this binder!" });

        if (page.Id is Guid pageId && _viewedPageIds.Add(pageId))
            _ = _pageRunService.MarkViewed(new(new() { Id = pageId }));
    }

    private Task PrefetchAdjacentPages()
    {
        if (Binder?.Pages is null || Binder.Pages.Count == 0)
            return Task.CompletedTask;

        List<Task> tasks = [];

        if (PageNumber > 1 && Binder.Pages[PageNumber - 2].Id is { } previousPageId)
            tasks.Add(PrefetchPage(previousPageId));

        if (PageNumber < Binder.Pages.Count && Binder.Pages[PageNumber].Id is { } nextPageId)
            tasks.Add(PrefetchPage(nextPageId));

        return tasks.Count == 0 ? Task.CompletedTask : Task.WhenAll(tasks);
    }

    private async Task PrefetchPage(Guid pageId)
    {
        if (_pageCache.ContainsKey(pageId))
            return;

        if (!_prefetchingPageIds.Add(pageId))
            return;

        try
        {
            var page = await FetchPage(pageId, false);

            if (page is not null)
                CachePage(pageId, page);
        }
        finally
        {
            _prefetchingPageIds.Remove(pageId);
        }
    }
}