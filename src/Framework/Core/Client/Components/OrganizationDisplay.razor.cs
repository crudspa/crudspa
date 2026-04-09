namespace Crudspa.Framework.Core.Client.Components;

public partial class OrganizationDisplay
{
    [Parameter, EditorRequired] public Organization? Organization { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
}