using Crudspa.Framework.Core.Shared.Extensions;
using Microsoft.AspNetCore.Components.Web;
using ButtonData = Crudspa.Content.Display.Shared.Contracts.Data.Button;

namespace Crudspa.Content.Display.Client.Components;

public partial class ButtonDisplay
{
    [Parameter, EditorRequired] public Button Button { get; set; } = null!;
    [Parameter] public EventCallback<ElementLink> ButtonClicked { get; set; }

    private Boolean HasText => Button.TextTypeIndex == ButtonData.TextTypes.Custom && Button.Text.HasSomething();
    private Boolean HasIconGraphic => Button.GraphicIndex == ButtonData.Graphics.Icon && Button.IconCssClass.HasSomething();
    private Boolean HasImageGraphic => Button.GraphicIndex == ButtonData.Graphics.Image && Button.ImageFile.Id.HasValue;
    private Boolean HasGraphic => HasIconGraphic || HasImageGraphic;
    private Boolean IsSquare => Button.ShapeIndex == ButtonData.Shapes.Square;
    private Boolean ShowLeadingGraphic => HasGraphic && (!HasText || IsSquare || Button.OrientationIndex == ButtonData.Orientations.Left);
    private Boolean ShowTrailingGraphic => HasGraphic && HasText && !IsSquare && Button.OrientationIndex == ButtonData.Orientations.Right;
    private String GraphicClass => HasImageGraphic ? "graphic image" : "graphic icon";
    private String ButtonImageUrl => Button.ImageFile.Id.HasValue ? Button.ImageFile.FetchUrl(192) : String.Empty;
    private String ButtonImageAlt => Button.ImageFile.Caption ?? Button.ImageFile.Name ?? Button.Text ?? String.Empty;
    private String ButtonClass => String.Join(" ", new[]
    {
        IsSquare ? "shape-square" : "shape-rectangle",
        HasText ? "with-text" : "without-text",
        HasGraphic ? "with-graphic" : "without-graphic",
        HasIconGraphic ? "with-icon" : String.Empty,
        HasImageGraphic ? "with-image" : String.Empty,
    }.Where(x => x.HasSomething()));
    private RenderFragment GraphicContent => builder =>
    {
        if (HasIconGraphic)
        {
            builder.OpenElement(0, "span");
            builder.AddAttribute(1, "class", Button.IconCssClass);
            builder.CloseElement();
            return;
        }

        if (!HasImageGraphic)
            return;

        builder.OpenElement(2, "img");
        builder.AddAttribute(3, "alt", ButtonImageAlt);
        builder.AddAttribute(4, "src", ButtonImageUrl);
        builder.AddAttribute(5, "draggable", "false");
        builder.CloseElement();
    };

    private String ButtonPath
    {
        get
        {
            if (Button.Internal == true)
                return $"/{Button.Path ?? String.Empty}";

            return $"https://{Button.Path ?? String.Empty}";
        }
    }

    private async Task HandleClick(MouseEventArgs args)
    {
        if (ButtonClicked.HasDelegate)
            await ButtonClicked.InvokeAsync(new() { Url = ButtonPath });
    }
}