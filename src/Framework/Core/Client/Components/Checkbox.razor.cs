namespace Crudspa.Framework.Core.Client.Components;

public partial class Checkbox
{
    [Parameter] public Boolean? Value { get; set; }
    [Parameter] public EventCallback<Boolean?> ValueChanged { get; set; }
    [Parameter] public EventCallback<Boolean?> AlsoValueChanged { get; set; }
    [Parameter] public Boolean ReadOnly { get; set; }
    [Parameter] public String? Label { get; set; }
    [Parameter] public String? ModifierClass { get; set; } = String.Empty;

    private async Task HandleChange(ChangeEventArgs args)
    {
        await ValueChanged.InvokeAsync(args.Value as Boolean?);
        await AlsoValueChanged.InvokeAsync(args.Value as Boolean?);
    }
}