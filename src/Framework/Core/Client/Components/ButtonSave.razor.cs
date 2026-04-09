namespace Crudspa.Framework.Core.Client.Components;

public partial class ButtonSave
{
    [Parameter, EditorRequired] public EventCallback Clicked { get; set; }
    [Parameter] public String Text { get; set; } = "Save";
}