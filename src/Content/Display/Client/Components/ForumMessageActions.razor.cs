namespace Crudspa.Content.Display.Client.Components;

public partial class ForumMessageActions
{
    [Parameter] public EventCallback ReplyClicked { get; set; }
    [Parameter] public EventCallback EditClicked { get; set; }
    [Parameter] public EventCallback DeleteRequested { get; set; }
    [Parameter] public EventCallback DeleteConfirmed { get; set; }
    [Parameter] public EventCallback DeleteCanceled { get; set; }
    [Parameter] public Boolean CanReply { get; set; }
    [Parameter] public Boolean CanEdit { get; set; }
    [Parameter] public Boolean CanDelete { get; set; }
    [Parameter] public Boolean ConfirmingDelete { get; set; }
    [Parameter] public Boolean Busy { get; set; }
}