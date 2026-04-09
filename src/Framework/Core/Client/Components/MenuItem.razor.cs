namespace Crudspa.Framework.Core.Client.Components;

public partial class MenuItem
{
    [Parameter, EditorRequired] public EventCallback Clicked { get; set; }
    [Parameter] public Types Type { get; set; } = Types.Custom;
    [Parameter] public String? Text { get; set; }
    [Parameter] public String? Icon { get; set; }
    [Parameter] public Boolean StartsGroup { get; set; }

    public enum Types
    {
        Custom,
        Reset,
        Refresh,
        Delete,
        Reorder,
    }

    public String ItemText
    {
        get
        {
            switch (Type)
            {
                case Types.Custom:
                    return Text ?? String.Empty;
                case Types.Reset:
                    return "Reset";
                case Types.Refresh:
                    return "Refresh";
                case Types.Delete:
                    return "Delete";
                case Types.Reorder:
                    return "Reorder";
                default:
                    throw new ArgumentOutOfRangeException(nameof(Type));
            }
        }
    }

    public String ItemIcon
    {
        get
        {
            switch (Type)
            {
                case Types.Custom:
                    return Icon ?? String.Empty;
                case Types.Reset:
                    return "c-icon-broom";
                case Types.Refresh:
                    return "c-icon-redo2";
                case Types.Delete:
                    return "c-icon-trash2";
                case Types.Reorder:
                    return "c-icon-arrows-sort";
                default:
                    throw new ArgumentOutOfRangeException(nameof(Type));
            }
        }
    }
}