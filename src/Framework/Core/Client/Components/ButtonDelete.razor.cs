namespace Crudspa.Framework.Core.Client.Components;

public partial class ButtonDelete
{
    [Parameter, EditorRequired] public EventCallback Clicked { get; set; }
}