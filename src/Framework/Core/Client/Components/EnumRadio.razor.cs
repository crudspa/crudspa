namespace Crudspa.Framework.Core.Client.Components;

public partial class EnumRadio<TEnum> : ComponentBase where TEnum : struct
{
    public enum Orientations { Vertical, Horizontal }

    [Parameter, EditorRequired] public TEnum SelectedValue { get; set; }
    [Parameter] public EventCallback<TEnum> SelectedValueChanged { get; set; }
    [Parameter] public Boolean ReadOnly { get; set; }
    [Parameter] public String? RadioCssClass { get; set; }
    [Parameter] public Orientations Orientation { get; set; } = Orientations.Horizontal;

    public readonly Guid InstanceId = Guid.NewGuid();

    protected void OnValueChanged(TEnum value)
    {
        if (SelectedValue.Equals(value)) return;
        SelectedValue = value;
        SelectedValueChanged.InvokeAsync(value);
    }
}