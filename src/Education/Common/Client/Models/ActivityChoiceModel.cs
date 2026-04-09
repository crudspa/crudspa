using Crudspa.Framework.Core.Client.Components;

namespace Crudspa.Education.Common.Client.Models;

public class ActivityChoiceModel(ActivityChoice choice) : Observable
{
    public enum States
    {
        Default,
        Selected,
        Valid,
        Invalid,
        Hidden,
        Locked,
    }

    public ActivityChoice Choice
    {
        get => choice;
        set => SetProperty(ref choice, value);
    }

    public States State
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? PositionStyle
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String StateClass
    {
        get
        {
            switch (State)
            {
                case States.Selected:
                    return "selected";
                case States.Valid:
                    return "valid";
                case States.Invalid:
                    return "invalid";
                case States.Hidden:
                    return "hidden";
                case States.Locked:
                    return "locked";
                case States.Default:
                default:
                    return "default";
            }
        }
    }

    public AudioPlayer? AudioPlayerElement { get; set; }
}