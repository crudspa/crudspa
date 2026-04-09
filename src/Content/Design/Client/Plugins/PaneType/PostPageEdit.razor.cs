namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class PostPageEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IPostService PostService { get; set; } = null!;

    public PostPageEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Id, ScrollService, PostService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class PostPageEditModel(Guid? id, IScrollService scrollService, IPostService postService)
    : EditModel<Post>(false)
{
    private void HandleBoxModelChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(BoxModel));

    public BoxModel? BoxModel
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Page? Page
    {
        get;
        set => SetProperty(ref field, value);
    }

    public async Task Initialize()
    {
        await Refresh();
    }

    public async Task Refresh()
    {
        ReadOnly = true;

        if (BoxModel is not null)
            await BoxModel.Hide();

        var idResponse = await WithWaiting("Fetching...", () => postService.FetchPageId(new(new() { Id = id })));

        if (idResponse.Ok)
        {
            Entity = idResponse.Value;

            var pageResponse = await WithWaiting("Fetching...", () => postService.FetchPage(new(new() { PostId = id })));

            if (pageResponse.Ok)
                SetPage(pageResponse.Value);
        }
    }

    public async Task Save()
    {
        await BoxModel!.Hide();
        await WithWaiting("Saving...", () => postService.SavePage(new(new() { PostId = id, Page = Page! })));
    }

    private void SetPage(Page page)
    {
        Page = page;

        if (BoxModel is not null)
            BoxModel.PropertyChanged -= HandleBoxModelChanged;

        BoxModel = new(scrollService, page.Box);
        BoxModel.PropertyChanged += HandleBoxModelChanged;
    }
}