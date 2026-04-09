namespace Crudspa.Education.Common.Client.Components;

public partial class PicButton
{
    [Parameter, EditorRequired] public EventCallback Clicked { get; set; }
    [Parameter, EditorRequired] public ActivityChoiceModel Model { get; set; } = null!;
    [Parameter] public Boolean Disabled { get; set; }
}