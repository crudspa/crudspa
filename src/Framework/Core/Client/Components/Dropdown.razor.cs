namespace Crudspa.Framework.Core.Client.Components;

public partial class Dropdown
{
    [Parameter, EditorRequired] public IEnumerable<INamed> LookupValues { get; set; } = null!;
    [Parameter] public Guid? Value { get; set; }
    [Parameter] public EventCallback<Guid?> ValueChanged { get; set; }
    [Parameter] public Boolean ReadOnly { get; set; }
    [Parameter] public Boolean AllowNull { get; set; }
    [Parameter] public String? NullText { get; set; } = "[None]";

    private async Task HandleValueChanged(ChangeEventArgs args)
    {
        if (args.Value is not null && Guid.TryParse(args.Value.ToString(), out var newValue))
        {
            Value = newValue;
            await ValueChanged.InvokeAsync(newValue);
        }
        else
        {
            Value = null;
            await ValueChanged.InvokeAsync(null);
        }
    }
}