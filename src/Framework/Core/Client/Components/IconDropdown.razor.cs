namespace Crudspa.Framework.Core.Client.Components;

public partial class IconDropdown
{
    [Parameter, EditorRequired] public IEnumerable<IconFull> LookupValues { get; set; } = null!;
    [Parameter] public Guid? Value { get; set; }
    [Parameter] public EventCallback<Guid?> ValueChanged { get; set; }
    [Parameter] public Boolean ReadOnly { get; set; }
    [Parameter] public Boolean AllowNull { get; set; }
    [Parameter] public String? NullText { get; set; } = "[None]";
}