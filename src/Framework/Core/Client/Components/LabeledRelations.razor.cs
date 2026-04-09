namespace Crudspa.Framework.Core.Client.Components;

public partial class LabeledRelations
{
    [Parameter, EditorRequired] public String Label { get; set; } = null!;
    [Parameter, EditorRequired] public IList<Selectable> Selectables { get; set; } = null!;
    [Parameter] public Boolean? All { get; set; }
    [Parameter] public String Width { get; set; } = "8em";
}