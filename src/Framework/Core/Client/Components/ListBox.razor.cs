using System.Reflection;

namespace Crudspa.Framework.Core.Client.Components;

public partial class ListBox<TItem>
    where TItem : class, INamed
{
    private static readonly PropertyInfo? DescriptionProperty = typeof(TItem).GetProperty(nameof(IDescribed.Description));

    [Parameter, EditorRequired] public IEnumerable<TItem> LookupValues { get; set; } = [];
    [Parameter] public Guid? Value { get; set; }
    [Parameter] public EventCallback<Guid?> ValueChanged { get; set; }
    [Parameter] public RenderFragment<TItem>? Template { get; set; }
    [Parameter] public Boolean ReadOnly { get; set; }
    [Parameter] public Boolean AllowFiltering { get; set; }
    [Parameter] public String? Style { get; set; }

    private Task HandleValueChanged(Guid? value)
    {
        Value = value;
        return ValueChanged.InvokeAsync(value);
    }

    private static String? GetName(TItem item) => item.Name;

    private static Boolean HasDescription(TItem item) => TryGetDescription(item).HasSomething();

    private static String? GetDescription(TItem item) => TryGetDescription(item);

    private static String? TryGetDescription(TItem item)
    {
        if (item is IDescribed described)
            return described.Description;

        if (DescriptionProperty?.PropertyType == typeof(String))
            return DescriptionProperty.GetValue(item) as String;

        return null;
    }
}