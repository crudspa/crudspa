namespace Crudspa.Framework.Core.Client.Components;

public partial class LabeledSelections
{
    [Parameter, EditorRequired] public String Label { get; set; } = null!;
    [Parameter, EditorRequired] public IList<Selectable> Selectables { get; set; } = null!;
    [Parameter] public String Width { get; set; } = "8em";
}