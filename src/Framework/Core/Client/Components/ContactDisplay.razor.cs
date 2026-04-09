namespace Crudspa.Framework.Core.Client.Components;

public partial class ContactDisplay
{
    [Parameter, EditorRequired] public Contact? Contact { get; set; }
    [Parameter, EditorRequired] public User? User { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
}