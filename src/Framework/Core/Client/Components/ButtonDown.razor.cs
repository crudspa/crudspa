namespace Crudspa.Framework.Core.Client.Components;

public partial class ButtonDown
{
    [Parameter, EditorRequired] public EventCallback Clicked { get; set; }
}