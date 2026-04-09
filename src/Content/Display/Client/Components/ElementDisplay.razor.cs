using System.Text;
using Crudspa.Content.Display.Client.Extensions;

namespace Crudspa.Content.Display.Client.Components;

public partial class ElementDisplay
{
    [Parameter, EditorRequired] public ElementDisplayModel Model { get; set; } = null!;

    public String ElementStyles()
    {
        var styles = new StringBuilder(String.Empty);

        if (Model.Element.ElementType?.OnlyChild == true || IsUnconstrainedVideoElement())
            styles.Append("width: 100%; ");

        if (IsUnconstrainedVideoElement())
            styles.Append("flex-basis: 100%; max-width: 100%; min-width: 0; box-sizing: border-box; ");

        styles.Append(Model.Element.Item.ItemStyles());
        styles.Append(Model.Element.Box.BoxStyles());

        return styles.ToString();
    }

    private Boolean IsUnconstrainedVideoElement()
    {
        var item = Model.Element.Item;

        return Model.Element.TypeId == Crudspa.Content.Display.Shared.Contracts.Ids.ElementTypeIds.Video
            && item.BasisId == Crudspa.Content.Display.Shared.Contracts.Ids.BasisIds.Auto
            && item.Width.HasNothing()
            && item.MaxWidth.HasNothing()
            && item.MinWidth.HasNothing();
    }
}