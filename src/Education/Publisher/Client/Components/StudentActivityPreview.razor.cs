namespace Crudspa.Education.Publisher.Client.Components;

public partial class StudentActivityPreview
{
    [Parameter, EditorRequired] public Activity Activity { get; set; } = null!;
    [Parameter] public Int32 Version { get; set; }
}