namespace Crudspa.Framework.Core.Client.Components;

public partial class CheckedList
{
    [Parameter, EditorRequired] public ObservableCollection<Selectable> Selectables { get; set; } = null!;
    [Parameter] public Boolean ReadOnly { get; set; }
    [Parameter] public Heights Height { get; set; } = Heights.Tall;
    [Parameter] public String WidthClass { get; set; } = String.Empty;

    public enum Heights
    {
        Short, Average, Tall, Full,
    }

    public String HeightClass => Height switch
    {
        Heights.Short => "short",
        Heights.Average => "average",
        Heights.Tall => "tall",
        Heights.Full => "full",
        _ => throw new ArgumentOutOfRangeException(nameof(Height)),
    };
}