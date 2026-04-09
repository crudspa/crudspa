namespace Crudspa.Framework.Core.Client.Components;

public partial class MaskedTextBox
{
    [Parameter, EditorRequired] public String? Mask { get; set; }
    [Parameter, EditorRequired] public String? Value { get; set; }
    [Parameter, EditorRequired] public String? CharacterPattern { get; set; }
    [Parameter, EditorRequired] public String? Placeholder { get; set; }
    [Parameter] public EventCallback<String?> ValueChanged { get; set; }
    [Parameter] public Boolean ReadOnly { get; set; }
}