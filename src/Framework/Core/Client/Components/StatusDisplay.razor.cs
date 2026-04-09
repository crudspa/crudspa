namespace Crudspa.Framework.Core.Client.Components;

public partial class StatusDisplay
{
    [Parameter] public String CssClass { get; set; } = String.Empty;
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public String? Name { get; set; }
    [Parameter] public String? ColorClass { get; set; }
    [Parameter] public Func<Guid?, String?, String?>? ColorClassFrom { get; set; }

    private String? ComputedColorClass => ColorClass.HasSomething()
        ? ColorClass
        : ColorClassFrom?.Invoke(Id, Name);

    private String ComputedCssClass
    {
        get
        {
            var classes = new System.Collections.Generic.List<String>();

            if (CssClass.HasSomething())
                classes.Add(CssClass);

            if (ComputedColorClass.HasSomething())
                classes.Add(ComputedColorClass!);

            return String.Join(" ", classes);
        }
    }
}