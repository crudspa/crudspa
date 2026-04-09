namespace Crudspa.Framework.Core.Client.Components;

public partial class ButtonCancel
{
    [Parameter, EditorRequired] public EventCallback Clicked { get; set; }
}