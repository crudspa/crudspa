namespace Crudspa.Framework.Core.Client.Components;

public partial class Radio
{
    [Parameter, EditorRequired] public IEnumerable<INamed> LookupValues { get; set; } = null!;
    [Parameter] public Guid? Value { get; set; }
    [Parameter] public EventCallback<Guid?> ValueChanged { get; set; }
    [Parameter] public Boolean ReadOnly { get; set; }
    [Parameter] public Boolean AllowNull { get; set; }
    [Parameter] public String? NullText { get; set; }
    [Parameter] public Boolean Vertical { get; set; }
    [Parameter] public String? RadioCssClass { get; set; }

    public readonly Guid InstanceId = Guid.NewGuid();

    private async Task HandleRadioChanged(Guid? id)
    {
        Value = id;
        await ValueChanged.InvokeAsync(Value);
    }

    private String WrapperClass => Vertical ? "c-stack c-padding-tiny-top" : "c-wrap top";
    private String RadioClass => $"{(Vertical ? "tight-bottom" : String.Empty)} {RadioCssClass}".Trim();
}