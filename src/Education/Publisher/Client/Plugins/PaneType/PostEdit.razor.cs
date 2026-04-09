using IPostService = Crudspa.Education.Publisher.Shared.Contracts.Behavior.IPostService;
using Post = Crudspa.Education.Publisher.Shared.Contracts.Data.Post;
using PostRemoved = Crudspa.Education.Publisher.Shared.Contracts.Events.PostRemoved;
using PostSaved = Crudspa.Education.Publisher.Shared.Contracts.Events.PostSaved;

namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class PostEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public ISessionState SessionState { get; set; } = null!;
    [Inject] public IPostService PostService { get; set; } = null!;

    public PostEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var forumId = Path!.Id("forum");

        Model = new(Path, Id, IsNew, forumId, EventBus, Navigator, SessionState, PostService);
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

public class PostEditModel : EditModel<Post>, IHandle<PostSaved>, IHandle<PostRemoved>, IHandle<PostReactionAdded>
{
    public enum MediaTypes
    {
        None,
        Audio,
        Image,
        Pdf,
        Video,
    }

    private readonly String? _path;
    private readonly Guid? _id;
    private readonly Guid? _forumId;
    private readonly INavigator _navigator;
    private readonly ISessionState _sessionState;
    private readonly IPostService _postService;

    public PostEditModel(String? path, Guid? id, Boolean isNew, Guid? forumId,
        IEventBus eventBus,
        INavigator navigator,
        ISessionState sessionState,
        IPostService postService) : base(isNew)
    {
        _path = path;
        _id = id;
        _forumId = forumId;
        _navigator = navigator;
        _sessionState = sessionState;
        _postService = postService;

        ReactionCharacters = Emoji.Reactions().Select(x => x.Character).ToList();

        eventBus.Subscribe(this);
    }

    public List<String> ReactionCharacters { get; set; }

    public async Task Handle(PostSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(PostRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public Task Handle(PostReactionAdded payload)
    {
        if (Entity is not null && _sessionState.ContactId.Equals(payload.ContactId) && _id.Equals(payload.PostId))
            Entity.ReactionCharacter = payload.Character;

        RaisePropertyChanged(nameof(Entity));

        return Task.CompletedTask;
    }

    public List<Orderable> GradeNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Orderable> ClassroomTypeNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchGradeNames(),
            FetchClassroomTypeNames());

        await Refresh();
    }

    public MediaTypes SelectedMediaType
    {
        get;
        set => SetProperty(ref field, value);
    } = MediaTypes.None;

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            SetPost(new()
            {
                ForumId = _forumId,
                Name = "New Post",
                Body = "<p><strong>Explain your activity or lesson:</strong></p><p></p><p><strong>What feedback would you like?</strong></p><p></p>",
                Pinned = false,
                Type = Post.PostTypes.InProcess,
            });
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _postService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
                SetPost(response.Value);
        }
    }

    public async Task Save()
    {
        DiscardUnusedMedia();

        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _postService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/post-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _postService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    public async Task AddReaction(String? character)
    {
        var postReaction = new PostReaction
        {
            PostId = _id,
            Character = character,
        };

        _ = await WithWaiting("Adding...", () => _postService.AddReaction(new(postReaction)));
    }

    public async Task FetchGradeNames()
    {
        var response = await WithAlerts(() => _postService.FetchGradeNames(new()), false);
        if (response.Ok) GradeNames = response.Value.ToList();
    }

    public async Task FetchClassroomTypeNames()
    {
        var response = await WithAlerts(() => _postService.FetchClassroomTypeNames(new()), false);
        if (response.Ok) ClassroomTypeNames = response.Value.ToList();
    }

    private void DiscardUnusedMedia()
    {
        if (Entity is null)
            return;

        switch (SelectedMediaType)
        {
            case MediaTypes.None:
                Entity.AudioFile = new();
                Entity.ImageFile = new();
                Entity.PdfFile = new();
                Entity.VideoFile = new();
                break;
            case MediaTypes.Audio:
                Entity.ImageFile = new();
                Entity.PdfFile = new();
                Entity.VideoFile = new();
                break;
            case MediaTypes.Image:
                Entity.AudioFile = new();
                Entity.PdfFile = new();
                Entity.VideoFile = new();
                break;
            case MediaTypes.Pdf:
                Entity.AudioFile = new();
                Entity.ImageFile = new();
                Entity.VideoFile = new();
                break;
            case MediaTypes.Video:
                Entity.AudioFile = new();
                Entity.ImageFile = new();
                Entity.PdfFile = new();
                break;
        }
    }

    private void SetPost(Post post)
    {
        if (post.AudioFile.Id.HasValue)
            SelectedMediaType = MediaTypes.Audio;
        else if (post.ImageFile.Id.HasValue)
            SelectedMediaType = MediaTypes.Image;
        else if (post.PdfFile.Id.HasValue)
            SelectedMediaType = MediaTypes.Pdf;
        else if (post.VideoFile.Id.HasValue)
            SelectedMediaType = MediaTypes.Video;
        else
            SelectedMediaType = MediaTypes.None;

        Entity = post;
        _navigator.UpdateTitle(_path, Entity.Name!);
    }
}