namespace Crudspa.Content.Display.Client.Components;

public partial class ForumReactionBar
{
    [Parameter, EditorRequired] public IEnumerable<CommentReaction> Reactions { get; set; } = [];
    [Parameter, EditorRequired] public IEnumerable<Emoji> Options { get; set; } = [];
    [Parameter, EditorRequired] public EventCallback<String?> Reacted { get; set; }
    [Parameter] public Boolean Busy { get; set; }
}