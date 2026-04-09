namespace Crudspa.Content.Design.Client.Components;

public partial class SectionEditView
{
    [Parameter, EditorRequired] public SectionEditModel Model { get; set; } = null!;
    [Parameter, EditorRequired] public Func<Task> CancelClicked { get; set; } = null!;
}