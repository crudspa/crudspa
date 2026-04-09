using IPostService = Crudspa.Education.School.Shared.Contracts.Behavior.IPostService;
using Post = Crudspa.Education.School.Shared.Contracts.Data.Post;
using PostAdded = Crudspa.Education.School.Shared.Contracts.Events.PostAdded;
using PostRemoved = Crudspa.Education.School.Shared.Contracts.Events.PostRemoved;
using PostSaved = Crudspa.Education.School.Shared.Contracts.Events.PostSaved;

namespace Crudspa.Education.School.Client.Components;

public partial class CommentTree : IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String Path { get; set; } = null!;
    [Parameter] public Guid? Id { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IPostService PostService { get; set; } = null!;

    public CommentTreeModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, PostService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class CommentTreeModel : ScreenModel, IHandle<PostAdded>, IHandle<PostSaved>, IHandle<PostRemoved>
{
    private readonly IEventBus _eventBus;
    private readonly IScrollService _scrollService;
    private readonly IPostService _postService;
    private readonly Guid? _parentId;

    private ObservableCollection<CommentEditModel> _commentModels = [];

    public CommentTreeModel(IEventBus eventBus,
        IScrollService scrollService,
        IPostService postService,
        Guid? parentId)
    {
        _eventBus = eventBus;
        _scrollService = scrollService;
        _postService = postService;
        _parentId = parentId;

        _eventBus.Subscribe(this);
    }

    public async Task Handle(PostAdded payload)
    {
        ObservableCollection<CommentEditModel>? collection = null;

        if (payload.ParentId == _parentId)
            collection = _commentModels;
        else
        {
            var parent = FindModel(_commentModels, payload.ParentId);
            if (parent is not null)
                collection = parent.Children;
        }

        if (collection is not null)
        {
            var comment = new Post
            {
                Id = payload.Id,
                ParentId = payload.ParentId,
            };

            var model = new CommentEditModel(_eventBus, _scrollService, _postService, comment, false);
            await model.Refresh();

            collection.Add(model);
            RaisePropertyChanged(nameof(CommentModels));
        }
    }

    public async Task Handle(PostSaved payload)
    {
        var model = FindModel(_commentModels, payload.Id);

        if (model is not null)
            await model.Refresh();
    }

    public Task Handle(PostRemoved payload)
    {
        ObservableCollection<CommentEditModel>? collection = null;

        if (payload.ParentId == _parentId)
            collection = _commentModels;
        else
        {
            var parent = FindModel(_commentModels, payload.ParentId);
            if (parent is not null)
                collection = parent.Children;
        }

        if (collection is not null)
        {
            var toRemove = collection.FirstOrDefault(x => x.Entity!.Id == payload.Id);

            if (toRemove is not null)
            {
                collection.Remove(toRemove);
                RaisePropertyChanged(nameof(CommentModels));
            }
        }

        return Task.CompletedTask;
    }

    private CommentEditModel? FindModel(ObservableCollection<CommentEditModel> models, Guid? id)
    {
        foreach (var model in models)
        {
            if (model.Entity!.Id == id)
                return model;

            var result = FindModel(model.Children, id);

            if (result is not null)
                return result;
        }

        return null;
    }

    public ObservableCollection<CommentEditModel> CommentModels
    {
        get => _commentModels;
        set => SetProperty(ref _commentModels, value);
    }

    public async Task Initialize()
    {
        await Refresh();
    }

    public async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request<Post>(new() { Id = _parentId });
        var response = await WithWaiting("Fetching...", () => _postService.FetchTreeForParent(request), resetAlerts);

        if (response.Ok)
            CommentModels = CreateCommentModels(response.Value.ToObservable());
    }

    private ObservableCollection<CommentEditModel> CreateCommentModels(ObservableCollection<Post> posts)
    {
        var commentModels = new ObservableCollection<CommentEditModel>();

        foreach (var post in posts)
        {
            var commentModel = new CommentEditModel(_eventBus, _scrollService, _postService, post, false);
            commentModel.Children = CreateCommentModels(post.Children);
            commentModels.Add(commentModel);
        }

        return commentModels;
    }

    public async Task AddComment()
    {
        var post = new Post
        {
            Id = Guid.NewGuid(),
            ParentId = _parentId,
            Pinned = false,
            Posted = DateTimeOffset.Now.AddHours(12),
        };

        CommentModels.Add(new(_eventBus, _scrollService, _postService, post, true));

        RaisePropertyChanged(nameof(CommentModels));

        await _scrollService.ToId(post.Id.GetValueOrDefault());
    }

    public Task Remove(Guid? id)
    {
        CommentModels.RemoveWhere(x => x.Entity!.Id.Equals(id));
        return Task.CompletedTask;
    }
}