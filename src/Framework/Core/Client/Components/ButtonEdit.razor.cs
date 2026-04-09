namespace Crudspa.Framework.Core.Client.Components;

public partial class ButtonEdit
{
    [Parameter, EditorRequired] public EventCallback Clicked { get; set; }
    [Parameter] public Boolean Disabled { get; set; }
    [Parameter] public Boolean Transparent { get; set; }
}