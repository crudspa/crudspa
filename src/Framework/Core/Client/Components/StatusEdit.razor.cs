namespace Crudspa.Framework.Core.Client.Components;

public partial class StatusEdit
{
    [Parameter] public String CssClass { get; set; } = String.Empty;
    [Parameter] public Func<Guid?, String?, String?>? ColorClassFrom { get; set; }
    [Parameter, EditorRequired] public IEnumerable<INamed> LookupValues { get; set; } = null!;
    [Parameter] public Guid? Value { get; set; }
    [Parameter] public EventCallback<Guid?> ValueChanged { get; set; }
    [Parameter] public Boolean ReadOnly { get; set; }
    [Parameter] public Boolean AllowNull { get; set; }
    [Parameter] public String? NullText { get; set; }

    public readonly Guid InstanceId = Guid.NewGuid();

    private async Task HandleRadioChanged(Guid? id)
    {
        Value = id;
        await ValueChanged.InvokeAsync(Value);
    }

    private String OptionClass(INamed? named, Guid? id, String? name)
    {
        var classes = new System.Collections.Generic.List<String>
        {
            "c-button",
            "transparent",
            "bordered",
            "label",
        };

        var colorClass = ResolveColorClass(named, id, name);

        if (colorClass.HasSomething())
            classes.Add(colorClass!);

        if (Value.Equals(id))
        {
            classes.Add("selected");
        }

        return String.Join(" ", classes);
    }

    private String? ResolveColorClass(INamed? named, Guid? id, String? name)
    {
        if (named is ICssClass cssClass && cssClass.CssClass.HasSomething())
            return cssClass.CssClass;

        return ColorClassFrom?.Invoke(id, name);
    }
}