namespace Crudspa.Framework.Core.Client.Components;

public partial class ButtonCore
{
    public enum IconPositions { Left, Right }

    public enum ButtonStyles
    {
        Default,
        Cancel,
        Create,
        Destroy,
        Edit,
        Media,
        Move,
        Primary,
        Reversed,
        Save,
        Transparent,
        View,
    }

    public enum Sizes { Default, Small, Large }

    [Parameter, EditorRequired] public EventCallback Clicked { get; set; }
    [Parameter] public String? Text { get; set; } = String.Empty;
    [Parameter] public String? IconClass { get; set; } = String.Empty;
    [Parameter] public Boolean Disabled { get; set; }
    [Parameter] public IconPositions IconPosition { get; set; } = IconPositions.Left;
    [Parameter] public ButtonStyles ButtonStyle { get; set; } = ButtonStyles.Default;
    [Parameter] public Sizes Size { get; set; } = Sizes.Default;

    public String ButtonClass
    {
        get
        {
            var buttonClass = String.Empty;

            switch (Size)
            {
                case Sizes.Large:
                    buttonClass += "large ";
                    break;
                case Sizes.Small:
                    buttonClass += "small ";
                    break;
            }

            switch (ButtonStyle)
            {
                case ButtonStyles.Cancel:
                    return buttonClass + "cancel";
                case ButtonStyles.Create:
                    return buttonClass + "create";
                case ButtonStyles.Destroy:
                    return buttonClass + "destroy";
                case ButtonStyles.Edit:
                    return buttonClass + "edit";
                case ButtonStyles.Media:
                    return buttonClass + "media";
                case ButtonStyles.Move:
                    return buttonClass + "move";
                case ButtonStyles.Primary:
                    return buttonClass + "primary";
                case ButtonStyles.Reversed:
                    return buttonClass + "reversed";
                case ButtonStyles.Save:
                    return buttonClass + "save";
                case ButtonStyles.Transparent:
                    return buttonClass + "transparent bordered";
                case ButtonStyles.View:
                    return buttonClass + "view";
                case ButtonStyles.Default:
                default:
                    return buttonClass;
            }
        }
    }
}