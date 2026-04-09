using Microsoft.AspNetCore.Components.Web;
using G = System.Collections.Generic;

namespace Crudspa.Framework.Core.Client.Components;

public partial class Image
{
    private static readonly Int32[] PredefinedWidths = [96, 192, 360, 540, 720, 1080, 1440, 1920, 3840];

    [Parameter, EditorRequired] public ImageFile? ImageFile { get; set; }
    [Parameter] public Int32? Width { get; set; }
    [Parameter] public String? Sizes { get; set; }
    [Parameter] public Boolean Obscured { get; set; }
    [Parameter] public Boolean Shadowed { get; set; }
    [Parameter] public String CssClass { get; set; } = String.Empty;
    [Parameter] public String? HyperlinkUrl { get; set; }
    [Parameter] public EventCallback<MouseEventArgs> HyperlinkClicked { get; set; }

    public readonly Guid InstanceId = Guid.NewGuid();
    public String DivStyles { get; private set; } = String.Empty;
    public String ImgSrc { get; private set; } = String.Empty;
    public String ImgSrcSet { get; private set; } = String.Empty;
    public String? ImgSizes { get; private set; }
    public readonly Dictionary<String, Object> ImgAttributes = new();

    private async Task HandleHyperlinkClicked(MouseEventArgs args)
    {
        if (HyperlinkClicked.HasDelegate)
            await HyperlinkClicked.InvokeAsync(args);
    }

    protected override void OnParametersSet()
    {
        if (ImageFile?.Id is null) return;

        ImgAttributes.Clear();

        var wrapperStyles = new G.List<String>
        {
            "min-width:0; min-height:0; box-sizing:border-box;",
        };

        if (ImageFile.Width is { } imageWidth && ImageFile.Height is { } imageHeight)
        {
            ImgAttributes["width"] = imageWidth;
            ImgAttributes["height"] = imageHeight;
        }

        if (Width is { } fixedWidth)
        {
            wrapperStyles.Add($"width:{fixedWidth:D}px;");
            wrapperStyles.Add("flex:0 0 auto;");
        }
        else
            wrapperStyles.Add("flex:1 1 0%;");

        DivStyles = String.Concat(wrapperStyles);

        ImgSrc = BuildSrc();
        ImgSrcSet = BuildSrcSet();
        ImgSizes = Sizes ?? (Width is { } specifiedWidth ? $"{specifiedWidth:D}px" : "100vw");
    }

    private String BuildSrc()
    {
        if (Width is { } specified)
        {
            var chosen = PredefinedWidths.FirstOrDefault(x => x >= specified);

            if (ImageFile?.Width is { } natural && chosen >= natural)
                return BuildUrl(null);

            return chosen > 0
                ? BuildUrl(chosen)
                : BuildUrl(PredefinedWidths[0]);
        }

        return BuildUrl(PredefinedWidths[0]);
    }

    private String BuildSrcSet()
    {
        var candidates = new G.List<Int32>();

        if (ImageFile?.Width is { } naturalWidth)
        {
            candidates.AddRange(PredefinedWidths.Where(x => x < naturalWidth));
            if (!candidates.Contains(naturalWidth)) candidates.Add(naturalWidth);
        }
        else
            candidates.AddRange(PredefinedWidths);

        candidates.Sort();

        var parts = new G.List<String>(candidates.Count);

        foreach (var candidateWidth in candidates)
        {
            var url = ImageFile?.Width is { } naturalImageWidth && candidateWidth == naturalImageWidth
                ? BuildUrl(null)
                : BuildUrl(candidateWidth);

            parts.Add($"{url} {candidateWidth}w");
        }

        return String.Join(", ", parts);
    }

    private String BuildUrl(Int32? width)
    {
        return ImageFile!.FetchUrl(width);
    }
}