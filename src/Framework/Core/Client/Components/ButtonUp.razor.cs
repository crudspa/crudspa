namespace Crudspa.Framework.Core.Client.Components;

public partial class ButtonUp
{
    [Parameter, EditorRequired] public EventCallback Clicked { get; set; }
}