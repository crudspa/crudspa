namespace Crudspa.Framework.Core.Client.Components;

public partial class BoolRadio
{
    public enum Orientations { Vertical, Horizontal }

    [Parameter, EditorRequired] public Boolean? SelectedValue { get; set; }
    [Parameter, EditorRequired] public EventCallback<Boolean?> SelectedValueChanged { get; set; }

    [Parameter] public String? TrueLabel { get; set; } = "True";
    [Parameter] public String? FalseLabel { get; set; } = "False";
    [Parameter] public Boolean ReadOnly { get; set; }
    [Parameter] public String? RadioCssClass { get; set; }
    [Parameter] public Orientations Orientation { get; set; } = Orientations.Horizontal;

    public readonly Guid InstanceId = Guid.NewGuid();

    protected void OnValueChanged(Boolean value)
    {
        if (SelectedValue.HasValue && SelectedValue.Value.Equals(value)) return;
        SelectedValue = value;
        SelectedValueChanged.InvokeAsync(value);
    }
}