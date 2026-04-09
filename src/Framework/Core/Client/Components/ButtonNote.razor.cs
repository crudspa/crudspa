namespace Crudspa.Framework.Core.Client.Components;

public partial class ButtonNote
{
    [Parameter, EditorRequired] public EventCallback Clicked { get; set; }
    [Parameter] public Boolean Disabled { get; set; }
}