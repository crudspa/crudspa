namespace Crudspa.Content.Display.Client.Components;

public partial class ForumComment
{
    [Parameter, EditorRequired] public Comment Comment { get; set; } = null!;
    [Parameter, EditorRequired] public Guid? ThreadId { get; set; }
    [Parameter, EditorRequired] public Guid? ForumId { get; set; }
    [Parameter, EditorRequired] public IEnumerable<ForumBundle> ForumBundles { get; set; } = [];
    [Parameter] public Int32 Depth { get; set; }
    [Parameter] public EventCallback Changed { get; set; }

    [Inject] public IForumRunService ForumRunService { get; set; } = null!;
    [Inject] public ISessionState SessionState { get; set; } = null!;

    public IReadOnlyList<Emoji> ReactionOptions { get; } = Emoji.Reactions();
    public Comment? Draft { get; set; }
    public Comment? Reply { get; set; }
    public Boolean Editing { get; set; }
    public Boolean Replying { get; set; }
    public Boolean Busy { get; set; }
    public Boolean ConfirmingDelete { get; set; }
    public String? ErrorMessage { get; set; }

    public void BeginEdit()
    {
        Draft = Comment.DeepClone();
        Draft.ForumBundles = ForumBundles.ToList().DeepClone().ToObservable();
        Editing = true;
        ErrorMessage = null;
    }

    public void CancelEdit()
    {
        Draft = null;
        Editing = false;
    }

    public void BeginReply()
    {
        Reply = new()
        {
            ThreadId = ThreadId,
            ParentId = Comment.Id,
            Body = String.Empty,
            ForumBundles = ForumBundles.ToList().DeepClone().ToObservable(),
        };
        Replying = true;
        ErrorMessage = null;
    }

    public void CancelReply()
    {
        Reply = null;
        Replying = false;
    }

    public async Task Save()
    {
        if (Draft is null) return;
        await Run(async () => await ForumRunService.SaveComment(new(Draft)));
        if (ErrorMessage.HasNothing())
        {
            CancelEdit();
            await Changed.InvokeAsync();
        }
    }

    public async Task AddReply()
    {
        if (Reply is null) return;
        await Run(async () => await ForumRunService.AddComment(new(Reply)));
        if (ErrorMessage.HasNothing())
        {
            CancelReply();
            await Changed.InvokeAsync();
        }
    }

    public async Task Delete()
    {
        await Run(async () => await ForumRunService.RemoveComment(new(new()
        {
            Id = Comment.Id,
            ThreadId = ThreadId,
            ParentId = Comment.ParentId,
        })));
        if (ErrorMessage.HasNothing()) await Changed.InvokeAsync();
    }

    public void RequestDelete() => ConfirmingDelete = true;

    public void CancelDelete() => ConfirmingDelete = false;

    public async Task React(String? emoji)
    {
        var selected = Comment.Reactions.FirstOrDefault(x => x.Selected)?.Emoji;
        var next = emoji.IsBasically(selected) ? null : emoji;

        await Run(async () => await ForumRunService.SetReaction(new(new()
        {
            CommentId = Comment.Id,
            Emoji = next,
        })));
        if (ErrorMessage.HasNothing()) await Changed.InvokeAsync();
    }

    private async Task Run(Func<Task<Response>> action)
    {
        Busy = true;
        ErrorMessage = null;
        try
        {
            var response = await action();
            if (!response.Ok) ErrorMessage = response.Errors.FirstOrDefault()?.Message ?? "The request could not be completed.";
        }
        finally
        {
            Busy = false;
        }
    }

    private async Task Run<T>(Func<Task<Response<T>>> action) where T : class?
    {
        Busy = true;
        ErrorMessage = null;
        try
        {
            var response = await action();
            if (!response.Ok) ErrorMessage = response.Errors.FirstOrDefault()?.Message ?? "The request could not be completed.";
        }
        finally
        {
            Busy = false;
        }
    }
}