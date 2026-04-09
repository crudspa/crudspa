namespace Crudspa.Framework.Core.Client.Components;

public partial class EnumDropdown<TEnum> : ComponentBase where TEnum : struct
{
    [Parameter, EditorRequired] public TEnum SelectedValue { get; set; }
    [Parameter] public EventCallback<TEnum> SelectedValueChanged { get; set; }
    [Parameter] public Boolean ReadOnly { get; set; }

    private async Task HandleValueChanged(ChangeEventArgs args)
    {
        if (args.Value is String valueString)
        {
            if (Enum.TryParse<TEnum>(valueString, out var value) && !SelectedValue.Equals(value))
            {
                SelectedValue = value;
                await SelectedValueChanged.InvokeAsync(value);
            }
        }
    }
}