using System.Globalization;
using System.Text.RegularExpressions;
using Crudspa.Content.Display.Client.Extensions;
using Crudspa.Content.Display.Shared.Contracts.Config.ElementType;
using Crudspa.Content.Display.Shared.Contracts.Data;
using Crudspa.Content.Display.Shared.Contracts.Ids;
using DataContainer = Crudspa.Content.Display.Shared.Contracts.Data.Container;

namespace Crudspa.Content.Design.Client.Components;

public partial class SectionShapePreview
{
    private enum TextAlignments
    {
        Left,
        Center,
        Right,
    }

    private sealed record Line(String Width, String ClassName);

    private const Decimal Scale = 0.5m;

    private static readonly Regex LengthRegex = new(@"(-?(?:\d+|\d*\.\d+))(rem|em|px|vw|vh)\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex H1Regex = new(@"<h1\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex H2Regex = new(@"<h2\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex H3ToH6Regex = new(@"<h[3-6]\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex ParagraphRegex = new(@"<p\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex ListItemRegex = new(@"<li\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex StrongRegex = new(@"<strong\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    [Parameter, EditorRequired] public Section Section { get; set; } = null!;
    [Parameter] public Boolean Tall { get; set; }

    private IReadOnlyList<SectionElement> OrderedElements => Section.Elements.OrderBy(x => x.Ordinal).ToList();

    private String SectionStyles() => CombineStyles(
        "width: 100%; height: 100%; min-width: 0; min-height: 0;",
        ScaleStyles(Section.Container.ContainerStyles()),
        ScaleStyles(Section.Box.BoxStyles()));

    private String ElementStyles(SectionElement element) => CombineStyles(
        "display: flex; flex-direction: column; min-width: 0; min-height: 0;",
        ScaleStyles(element.Item.ItemStyles()),
        ScaleStyles(element.Box.BoxStyles()));

    private static String MultimediaContainerStyles(DataContainer container) => CombineStyles(
        "display: flex; flex: 1 1 auto; min-width: 0; min-height: 0;",
        ScaleStyles(container.ContainerStyles()));

    private static String MultimediaItemStyles(MultimediaItem item) => CombineStyles(
        "display: flex; flex-direction: column; min-width: 0; min-height: 0;",
        ScaleStyles(item.Item.ItemStyles()),
        ScaleStyles(item.Box.BoxStyles()));

    private static String ButtonStyles(Button button) => CombineStyles(
        "display: inline-flex; align-items: center; justify-content: center; gap: .35rem; min-width: 0;",
        ScaleStyles(button.Box.BoxStyles()));

    private static String PreviewBoxClasses(Box box)
    {
        var classes = new List<String>();

        if (HasPreviewChrome(box))
            classes.Add("preview-chrome");

        if (HasBorder(box))
            classes.Add("preview-bordered");

        if (HasRounded(box))
            classes.Add("preview-rounded");

        if (HasShadow(box))
            classes.Add("preview-shadowed");

        return String.Join(" ", classes);
    }

    private static Boolean ButtonHasText(Button button) =>
        button.TextTypeIndex == Button.TextTypes.Custom && button.Text.HasSomething();

    private static Boolean ButtonHasIcon(Button button) =>
        button.GraphicIndex == Button.Graphics.Icon && button.IconCssClass.HasSomething();

    private static Boolean ButtonHasImage(Button button) =>
        button.GraphicIndex == Button.Graphics.Image && button.ImageFile.Id.HasValue;

    private static Boolean ButtonHasGraphic(Button button) => ButtonHasIcon(button) || ButtonHasImage(button);

    private static Boolean ButtonShowsGraphicFirst(Button button) =>
        ButtonHasGraphic(button)
        && (!ButtonHasText(button)
            || button.ShapeIndex == Button.Shapes.Square
            || button.OrientationIndex == Button.Orientations.Left);

    private static Boolean ButtonShowsGraphicLast(Button button) =>
        ButtonHasGraphic(button)
        && ButtonHasText(button)
        && button.ShapeIndex == Button.Shapes.Rectangle
        && button.OrientationIndex == Button.Orientations.Right;

    private static String ButtonClassName(Button button)
    {
        var classes = new List<String> { "c-button", "c-shape-button" };

        classes.Add(button.ShapeIndex == Button.Shapes.Square ? "shape-square" : "shape-rectangle");
        classes.Add(ButtonHasText(button) ? "with-text" : "without-text");
        classes.Add(ButtonHasGraphic(button) ? "with-graphic" : "without-graphic");

        if (ButtonHasIcon(button))
            classes.Add("with-icon");

        if (ButtonHasImage(button))
            classes.Add("with-image");

        if (button.Box.BoxClasses().HasSomething())
            classes.Add(button.Box.BoxClasses());

        return String.Join(" ", classes);
    }

    private static String CombineStyles(params String?[] values) =>
        String.Join(" ", values.Where(x => x.HasSomething()));

    private static Boolean HasPreviewChrome(Box box) => HasBorder(box) || HasRounded(box) || HasShadow(box);

    private static Boolean HasBorder(Box box) =>
        box.BorderThickness.HasSomething()
        || box.BorderThicknessTop.HasSomething()
        || box.BorderThicknessLeft.HasSomething()
        || box.BorderThicknessRight.HasSomething()
        || box.BorderThicknessBottom.HasSomething();

    private static Boolean HasRounded(Box box) => box.BorderRadius.HasSomething();

    private static Boolean HasShadow(Box box) =>
        box.ShadowBlurRadius.HasSomething()
        || box.ShadowColor.HasSomething()
        || box.ShadowOffsetX.HasSomething()
        || box.ShadowOffsetY.HasSomething()
        || box.ShadowSpreadRadius.HasSomething();

    private static String ScaleStyles(String? styles)
    {
        if (styles.HasNothing())
            return String.Empty;

        return LengthRegex.Replace(styles!, match =>
        {
            var number = Decimal.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
            var scaled = number * Scale;
            var unit = match.Groups[2].Value;
            return $"{scaled.ToString("0.###", CultureInfo.InvariantCulture)}{unit}";
        });
    }

    private RenderFragment RenderElementBody(SectionElement element) => builder =>
    {
        if (element.TypeId == ElementTypeIds.TextElement && element.As<TextElement>() is { } textElement)
        {
            builder.AddContent(0, RenderTextPlaceholder(textElement.Text));
            return;
        }

        if (element.TypeId == ElementTypeIds.Image)
        {
            builder.AddContent(1, RenderMediaPlaceholder("c-icon-file-image", "image"));
            return;
        }

        if (element.TypeId == ElementTypeIds.Video)
        {
            builder.AddContent(2, RenderMediaPlaceholder("c-icon-play-circle", "video"));
            return;
        }

        if (element.TypeId == ElementTypeIds.Audio)
        {
            builder.AddContent(3, RenderAudioPlaceholder());
            return;
        }

        if (element.TypeId == ElementTypeIds.Button && element.As<ButtonElement>() is { } buttonElement)
        {
            builder.AddContent(4, RenderButtonPlaceholder(buttonElement.Button));
            return;
        }

        if (element.TypeId == ElementTypeIds.Multimedia && element.As<MultimediaElement>() is { } multimediaElement)
        {
            builder.AddContent(5, RenderMultimediaPlaceholder(multimediaElement));
            return;
        }

        if (element.TypeId == ElementTypeIds.Pdf)
        {
            builder.AddContent(6, RenderMediaPlaceholder("c-icon-file-pdf", "document"));
            return;
        }

        if (element.TypeId == ElementTypeIds.Note)
        {
            builder.AddContent(7, RenderMediaPlaceholder("c-icon-note", "note"));
            return;
        }

        builder.AddContent(8, RenderFallbackPlaceholder());
    };

    private RenderFragment RenderMultimediaPlaceholder(MultimediaElement multimediaElement) => builder =>
    {
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", "c-shape-flow");
        builder.AddAttribute(2, "style", MultimediaContainerStyles(multimediaElement.Container));

        var sequence = 3;

        foreach (var item in multimediaElement.MultimediaItems.OrderBy(x => x.Ordinal))
        {
            builder.OpenElement(sequence++, "div");
            builder.AddAttribute(sequence++, "class", $"c-shape-node {item.Box.BoxClasses()} {PreviewBoxClasses(item.Box)}");
            builder.AddAttribute(sequence++, "style", MultimediaItemStyles(item));
            builder.OpenElement(sequence++, "div");
            builder.AddAttribute(sequence++, "class", "c-shape-node-body");
            builder.AddContent(sequence++, RenderMultimediaItemBody(item));
            builder.CloseElement();
            builder.CloseElement();
        }

        builder.CloseElement();
    };

    private RenderFragment RenderMultimediaItemBody(MultimediaItem item) => builder =>
    {
        switch (item.MediaTypeIndex)
        {
            case MultimediaItem.MediaTypes.Text:
                builder.AddContent(0, RenderTextPlaceholder(item.Text));
                return;
            case MultimediaItem.MediaTypes.Image:
                builder.AddContent(1, RenderMediaPlaceholder("c-icon-file-image", "image"));
                return;
            case MultimediaItem.MediaTypes.Video:
                builder.AddContent(2, RenderMediaPlaceholder("c-icon-play-circle", "video"));
                return;
            case MultimediaItem.MediaTypes.Audio:
                builder.AddContent(3, RenderAudioPlaceholder());
                return;
            case MultimediaItem.MediaTypes.Button:
                builder.AddContent(4, RenderButtonPlaceholder(item.Button));
                return;
            default:
                builder.AddContent(5, RenderFallbackPlaceholder());
                return;
        }
    };

    private RenderFragment RenderTextPlaceholder(String? html) => builder =>
    {
        var alignment = DetermineAlignment(html);
        var alignmentClass = alignment switch
        {
            TextAlignments.Center => "center",
            TextAlignments.Right => "right",
            _ => "left",
        };

        var className = $"c-shape-copy {alignmentClass}";

        if (IsQuote(html))
            className += " quote";

        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", className);

        var sequence = 2;

        foreach (var line in BuildTextLines(html))
        {
            builder.OpenElement(sequence++, "div");
            builder.AddAttribute(sequence++, "class", $"c-shape-line {line.ClassName}");
            builder.AddAttribute(sequence++, "style", LineStyle(line.Width, alignment));
            builder.CloseElement();
        }

        builder.CloseElement();
    };

    private RenderFragment RenderButtonPlaceholder(Button button) => builder =>
    {
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", ButtonClassName(button));
        builder.AddAttribute(2, "style", ButtonStyles(button));

        var sequence = 3;

        if (ButtonShowsGraphicFirst(button))
        {
            builder.OpenElement(sequence++, "span");
            builder.AddAttribute(sequence++, "class", ButtonHasImage(button) ? "graphic image" : "graphic icon");
            builder.OpenElement(sequence++, "span");
            builder.AddAttribute(sequence++, "class", ButtonHasImage(button) ? "c-icon-file-image" : button.IconCssClass);
            builder.CloseElement();
            builder.CloseElement();
        }

        if (ButtonHasText(button))
        {
            builder.OpenElement(sequence++, "span");
            builder.AddAttribute(sequence++, "class", "text");
            builder.AddContent(sequence++, button.Text ?? "Button");
            builder.CloseElement();
        }

        if (ButtonShowsGraphicLast(button))
        {
            builder.OpenElement(sequence++, "span");
            builder.AddAttribute(sequence++, "class", "graphic icon");
            builder.OpenElement(sequence++, "span");
            builder.AddAttribute(sequence++, "class", button.IconCssClass);
            builder.CloseElement();
            builder.CloseElement();
        }

        builder.CloseElement();
    };

    private static RenderFragment RenderMediaPlaceholder(String iconCssClass, String variant) => builder =>
    {
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", $"c-shape-media {variant}");
        builder.OpenElement(2, "span");
        builder.AddAttribute(3, "class", $"icon {iconCssClass}");
        builder.CloseElement();
        builder.CloseElement();
    };

    private static RenderFragment RenderAudioPlaceholder() => builder =>
    {
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", "c-shape-media audio");

        builder.OpenElement(2, "span");
        builder.AddAttribute(3, "class", "icon c-icon-volume-high");
        builder.CloseElement();

        builder.OpenElement(4, "div");
        builder.AddAttribute(5, "class", "wave");

        builder.OpenElement(6, "div");
        builder.AddAttribute(7, "class", "bar a");
        builder.CloseElement();

        builder.OpenElement(8, "div");
        builder.AddAttribute(9, "class", "bar b");
        builder.CloseElement();

        builder.OpenElement(10, "div");
        builder.AddAttribute(11, "class", "bar c");
        builder.CloseElement();

        builder.CloseElement();
        builder.CloseElement();
    };

    private static RenderFragment RenderFallbackPlaceholder() => builder =>
    {
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", "c-shape-fallback");
        builder.OpenElement(2, "span");
        builder.AddAttribute(3, "class", "icon c-icon-shape");
        builder.CloseElement();
        builder.CloseElement();
    };

    private static TextAlignments DetermineAlignment(String? html)
    {
        if (html.HasNothing())
            return TextAlignments.Left;

        if (html!.Contains("text-align: center", StringComparison.OrdinalIgnoreCase)
            || html.Contains("align=\"center\"", StringComparison.OrdinalIgnoreCase))
            return TextAlignments.Center;

        if (html.Contains("text-align: right", StringComparison.OrdinalIgnoreCase)
            || html.Contains("align=\"right\"", StringComparison.OrdinalIgnoreCase))
            return TextAlignments.Right;

        return TextAlignments.Left;
    }

    private static Boolean IsQuote(String? html) =>
        html.HasSomething() && html!.Contains("<blockquote", StringComparison.OrdinalIgnoreCase);

    private static IReadOnlyList<Line> BuildTextLines(String? html)
    {
        var source = html ?? String.Empty;
        var lines = new List<Line>();

        if (IsQuote(source))
        {
            lines.Add(new("92%", "body"));
            lines.Add(new("100%", "body"));
            lines.Add(new("82%", "body"));

            if (StrongRegex.IsMatch(source))
            {
                lines.Add(new("46%", "meta"));
                lines.Add(new("34%", "meta"));
            }

            return lines;
        }

        var h1Count = H1Regex.Matches(source).Count;
        var h2Count = H2Regex.Matches(source).Count;
        var h3Count = H3ToH6Regex.Matches(source).Count;
        var paragraphCount = ParagraphRegex.Matches(source).Count;
        var listItemCount = ListItemRegex.Matches(source).Count;

        for (var index = 0; index < h1Count; index++)
            lines.Add(new(index == 0 ? "82%" : "68%", "display"));

        for (var index = 0; index < h2Count; index++)
            lines.Add(new(index % 2 == 0 ? "74%" : "58%", "heading"));

        for (var index = 0; index < h3Count; index++)
            lines.Add(new(index % 2 == 0 ? "64%" : "54%", "heading"));

        if (lines.Count == 0 && paragraphCount == 0 && listItemCount == 0)
        {
            lines.Add(new("72%", "heading"));
            paragraphCount = 1;
        }

        for (var index = 0; index < Math.Min(paragraphCount, 3); index++)
        {
            lines.Add(new("100%", "body"));
            lines.Add(new("92%", "body"));

            if (lines.Count < 7)
                lines.Add(new("72%", "body"));
        }

        for (var index = 0; index < Math.Min(listItemCount, 4); index++)
            lines.Add(new(index % 2 == 0 ? "88%" : "78%", "bullet"));

        if (StrongRegex.IsMatch(source) && lines.Count < 8)
            lines.Add(new("46%", "meta"));

        return lines.Take(8).ToList();
    }

    private static String LineStyle(String width, TextAlignments alignment)
    {
        return alignment switch
        {
            TextAlignments.Center => $"width: {width}; margin-left: auto; margin-right: auto;",
            TextAlignments.Right => $"width: {width}; margin-left: auto;",
            _ => $"width: {width};",
        };
    }
}