namespace Crudspa.Education.Common.Client.Components;

public partial class ChoiceButton
{
    [Parameter, EditorRequired] public EventCallback Clicked { get; set; }
    [Parameter, EditorRequired] public ActivityChoiceModel Model { get; set; } = null!;
    [Parameter] public Boolean Disabled { get; set; }
    [Parameter] public EventCallback<MediaPlay> MediaPlayed { get; set; }
}