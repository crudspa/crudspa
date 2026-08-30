namespace Crudspa.Framework.Core.Client.Components;

public partial class KeyDropdown
{
    [Parameter, EditorRequired] public IReadOnlyDictionary<String, String> LookupValues { get; set; } = null!;
    [Parameter] public String? Value { get; set; }
    [Parameter] public EventCallback<String?> ValueChanged { get; set; }
    [Parameter] public Boolean ReadOnly { get; set; }
    [Parameter] public Boolean AllowNull { get; set; }
    [Parameter] public String NullText { get; set; } = "[None]";

    private async Task HandleValueChanged(ChangeEventArgs args)
    {
        Value = args.Value?.ToString();
        await ValueChanged.InvokeAsync(Value);
    }
}