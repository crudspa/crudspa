namespace Crudspa.Framework.Core.Client.Components;

public partial class ButtonOk
{
    [Parameter, EditorRequired] public EventCallback Clicked { get; set; }
    [Parameter] public String OkText { get; set; } = "Ok";
}