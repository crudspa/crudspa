namespace Crudspa.Education.Publisher.Client.Components;

public partial class GuidePageEdit
{
    [Parameter, EditorRequired] public Page Page { get; set; } = null!;
    [Parameter] public Boolean ReadOnly { get; set; }
}