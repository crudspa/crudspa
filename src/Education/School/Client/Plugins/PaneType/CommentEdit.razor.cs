using IPostService = Crudspa.Education.School.Shared.Contracts.Behavior.IPostService;
using Post = Crudspa.Education.School.Shared.Contracts.Data.Post;

namespace Crudspa.Education.School.Client.Plugins.PaneType;

public partial class CommentEdit : IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter, EditorRequired] public String Path { get; set; } = null!;
    [Parameter, EditorRequired] public CommentEditModel Model { get; set; } = null!;
    [Parameter] public EventCallback<Guid?> RemoveRequested { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public ISessionState SessionState { get; set; } = null!;
    [Inject] public IPostService PostService { get; set; } = null!;
    [Inject] public ILogger<CommentEdit> Logger { get; set; } = null!;

    protected override Task OnInitializedAsync()
    {
        Model.PropertyChanged += HandleModelChanged;
        Model.RemoveRequested += HandleRemoveSelected;
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.RemoveRequested -= HandleRemoveSelected;
        Model.Dispose();
    }

    private async Task HandleCancelClicked()
    {
        if (Model.IsNew)
            await RemoveRequested.InvokeAsync(Model.Entity!.Id);
        else
            await Model.Refresh();
    }

    private async void HandleRemoveSelected(Object? sender, Guid id)
    {
        try
        {
            await RemoveRequested.InvokeAsync(id);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Exception in event handler.");
        }
    }
}

public class CommentEditModel : EditModel<Post>
{
    public enum MediaTypes
    {
        None,
        Audio,
        Image,
        Pdf,
        Video,
    }

    public event EventHandler<Guid>? RemoveRequested;

    private readonly IEventBus _eventBus;
    private readonly IScrollService _scrollService;
    private readonly IPostService _postService;

    public ModalModel ConfirmationModel { get; }

    public CommentEditModel(IEventBus eventBus,
        IScrollService scrollService,
        IPostService postService,
        Post comment,
        Boolean isNew)
        : base(isNew)
    {
        _eventBus = eventBus;
        _scrollService = scrollService;
        _postService = postService;

        if (isNew)
            ReadOnly = false;

        SetComment(comment);

        ConfirmationModel = new(scrollService);

        eventBus.Subscribe(this);
    }

    public ObservableCollection<CommentEditModel> Children
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public MediaTypes SelectedMediaType
    {
        get;
        set => SetProperty(ref field, value);
    } = MediaTypes.None;

    public Boolean CanEdit
    {
        get;
        set => SetProperty(ref field, value);
    } = true;

    public void SetMediaType(String selectedMediaType)
    {
        SelectedMediaType = Enum.Parse<MediaTypes>(selectedMediaType);
    }

    public async Task Refresh()
    {
        ReadOnly = true;

        var response = await WithWaiting("Fetching...", () => _postService.Fetch(new(new() { Id = Entity!.Id })));

        if (response.Ok)
            Entity = response.Value;
    }

    public async Task Reply()
    {
        var post = new Post
        {
            Id = Guid.NewGuid(),
            ParentId = Entity!.Id,
            Pinned = false,
            Posted = DateTimeOffset.Now.AddHours(12),
        };

        Children.Add(new(_eventBus, _scrollService, _postService, post, true));

        RaisePropertyChanged(nameof(Children));

        await _scrollService.ToId(post.Id.GetValueOrDefault());
    }

    public async Task Save()
    {
        DiscardUnusedMedia();

        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _postService.Add(new(Entity!)));

            if (response.Ok)
                RaiseRemoveRequested(Entity!.Id!.Value);
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _postService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    private void RaiseRemoveRequested(Guid id)
    {
        var raiseEvent = RemoveRequested;
        raiseEvent?.Invoke(this, id);
    }

    public async Task Delete()
    {
        _ = await WithWaiting("Deleting...", () => _postService.Remove(new(Entity!)));
    }

    public Task Remove(Guid? id)
    {
        Children.RemoveWhere(x => x.Entity!.Id.Equals(id));
        RaisePropertyChanged(nameof(Children));
        return Task.CompletedTask;
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

    private void SetComment(Post comment)
    {
        if (comment.AudioFile.Id.HasValue)
            SelectedMediaType = MediaTypes.Audio;
        else if (comment.ImageFile.Id.HasValue)
            SelectedMediaType = MediaTypes.Image;
        else if (comment.PdfFile.Id.HasValue)
            SelectedMediaType = MediaTypes.Pdf;
        else if (comment.VideoFile.Id.HasValue)
            SelectedMediaType = MediaTypes.Video;
        else
            SelectedMediaType = MediaTypes.None;

        Entity = comment;
    }
}