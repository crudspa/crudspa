namespace Crudspa.Framework.Core.Client.Components;

public partial class DateFilter
{
    [Parameter, EditorRequired] public DateRange Range { get; set; } = null!;
}