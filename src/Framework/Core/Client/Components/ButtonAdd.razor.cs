namespace Crudspa.Framework.Core.Client.Components;

public partial class ButtonAdd
{
    [Parameter, EditorRequired] public EventCallback Clicked { get; set; }
    [Parameter] public String Text { get; set; } = "Add";
    [Parameter] public String? EntityName { get; set; } = String.Empty;
    [Parameter] public Boolean Disabled { get; set; }
    [Parameter] public Boolean Transparent { get; set; }
}