namespace Crudspa.Framework.Core.Client.Components;

public partial class Waiter
{
    [Parameter, EditorRequired] public RenderFragment ChildContent { get; set; } = null!;
    [Parameter, EditorRequired] public ScreenModel Model { get; set; } = null!;
}