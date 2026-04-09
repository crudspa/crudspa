namespace Crudspa.Framework.Core.Client.Components;

public partial class UsaPostalDisplay
{
    [Parameter, EditorRequired] public UsaPostal? UsaPostal { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
}