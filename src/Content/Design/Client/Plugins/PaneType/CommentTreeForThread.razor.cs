
using Thread = Crudspa.Content.Display.Shared.Contracts.Data.Thread;
using Comment = Crudspa.Content.Display.Shared.Contracts.Data.Comment;
namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class CommentTreeForThread : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public ICommentService CommentService { get; set; } = null!;

    public CommentTreeForThreadModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, CommentService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class CommentTreeForThreadModel : TreeModel<CommentTreeForThreadItem>,
    IHandle<CommentAdded>, IHandle<CommentSaved>, IHandle<CommentRemoved>
{
    private readonly ICommentService _commentService;
    private readonly Guid? _threadId;
    private void HandleBatchModelChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(args.PropertyName);
    public BatchModel<CommentMedia> CommentMediasModel { get; } = new();

    public CommentTreeForThreadModel(IEventBus eventBus, IScrollService scrollService, ICommentService commentService, Guid? threadId)
        : base(scrollService, x => x.Children)
    {
        _commentService = commentService;

        _threadId = threadId;
        CommentMediasModel.PropertyChanged += HandleBatchModelChanged;
        eventBus.Subscribe(this);
    }

    public override void Dispose()
    {
        CommentMediasModel.PropertyChanged -= HandleBatchModelChanged;

        base.Dispose();
    }

    public async Task Handle(CommentAdded payload)
    {
        if (payload.ThreadId.Equals(_threadId))
            await Refresh();
    }

    public async Task Handle(CommentSaved payload)
    {
        if (payload.ThreadId.Equals(_threadId))
            await Refresh();
    }

    public async Task Handle(CommentRemoved payload)
    {
        if (payload.ThreadId.Equals(_threadId))
            await Refresh();
    }


    public async Task Initialize()
    {
        await Refresh();
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request<Thread>(new() { Id = _threadId });
        var response = await WithWaiting("Fetching...", () => _commentService.FetchTreeForThread(request), resetAlerts);

        if (response.Ok)
            SetRoots(response.Value.Select(x => new CommentTreeForThreadItem(x)).ToList());
    }

    public override async Task Create(Guid? parentId)
    {
        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            Body = String.Empty,
            ThreadId = _threadId,
            ParentId = parentId,
        };

        await CreateNode(new CommentTreeForThreadItem(comment), parentId);
    }

    public BatchModel<CommentMedia> GetCommentMediasModel(Guid? id)
    {
        BindCommentMediasModel(id);
        return CommentMediasModel;
    }

    public void AddCommentMedia(Guid? id)
    {
        BindCommentMediasModel(id);

        CommentMediasModel.Entities.Add(new()
        {
            Id = Guid.NewGuid(),
            CommentId = id,
            Ordinal = CommentMediasModel.Entities.Count,
        });
    }

    public void SetCommentMediaType(Guid? id, CommentMedia media, CommentMedia.Types type)
    {
        BindCommentMediasModel(id);

        media.AudioFile = new();
        media.ImageFile = new();
        media.PdfFile = new();
        media.VideoFile = new();
        media.Type = type;

        RaisePropertyChanged(nameof(CommentMediasModel));
    }

    private void BindCommentMediasModel(Guid? id)
    {
        var node = FindNode(id);
        if (node is null)
            return;

        CommentMediasModel.Entities = node.Entity.Comment.CommentMedias;
    }

    public override async Task<Response<CommentTreeForThreadItem?>> Add(FormModel<CommentTreeForThreadItem> form)
    {
        var response = await _commentService.Add(new(form.Entity.Comment));

        return response.Ok
            ? new(new CommentTreeForThreadItem(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response> Save(FormModel<CommentTreeForThreadItem> form)
    {
        return await _commentService.Save(new(form.Entity.Comment));
    }

    public override async Task<Response> Remove(Guid? id)
    {
        var node = FindNode(id);

        if (node is null)
            return new();

        return await _commentService.Remove(new(new()
        {
            Id = id,
            ThreadId = _threadId,
            ParentId = node.Entity.Comment.ParentId,
        }));
    }


    public override Boolean InScope(Guid? scopeIdValue)
    {
        return scopeIdValue is null || scopeIdValue.Equals(_threadId);
    }

}

public class CommentTreeForThreadItem : Observable, IDisposable, INamed
{
    private void HandleCommentChanged(Object? sender, PropertyChangedEventArgs args)
    {
        RaisePropertyChanged(nameof(Comment));
        RaisePropertyChanged(nameof(Name));
    }

    private Comment _comment;

    public String? Name => Crudspa.Framework.Core.Shared.Markup.ProseHtmlNormalizer.ToPlainTextSummary(Comment.Body);

    public CommentTreeForThreadItem(Comment comment)
    {
        _comment = comment;
        _comment.PropertyChanged += HandleCommentChanged;

        if (_comment.Children.HasItems())
            Children = _comment.Children.Select(x => new CommentTreeForThreadItem(x)).ToObservable();
    }

    public void Dispose()
    {
        _comment.PropertyChanged -= HandleCommentChanged;

        foreach (var child in Children)
            child.Dispose();
    }

    public Guid? Id
    {
        get => _comment.Id;
        set => _comment.Id = value;
    }

    public ObservableCollection<CommentTreeForThreadItem> Children
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public Comment Comment
    {
        get => _comment;
        set => SetProperty(ref _comment, value);
    }
}