namespace Crudspa.Content.Design.Client.Components;

public partial class PageEdit
{
    [Parameter, EditorRequired] public PageEditModelBase Model { get; set; } = null!;
    [Parameter, EditorRequired] public Func<Task> CancelClicked { get; set; } = null!;
}