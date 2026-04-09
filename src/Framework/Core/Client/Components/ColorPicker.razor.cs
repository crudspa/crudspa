using System.Globalization;

namespace Crudspa.Framework.Core.Client.Components;

public partial class ColorPicker
{
    [Parameter, EditorRequired] public String? Value { get; set; }
    [Parameter] public EventCallback<String?> ValueChanged { get; set; }
    [Parameter] public Boolean ReadOnly { get; set; }

    public String? Rgba { get; private set; }

    protected override void OnParametersSet()
    {
        Rgba = HexToRgba(Value);
    }

    private async Task RgbaChanged(String? rgba)
    {
        Rgba = rgba;
        await ValueChanged.InvokeAsync(RgbaToHex(rgba));
    }

    private static String? HexToRgba(String? hex)
    {
        if (hex.HasNothing())
            return null;

        hex = hex.TrimStart('#');

        if (hex.Length is 3 or 4)
            hex = String.Concat(hex.Select(x => $"{x}{x}"));

        if (hex.Length is not (6 or 8))
            return null;

        var r = Convert.ToByte(hex[..2], 16);
        var g = Convert.ToByte(hex.Substring(2, 2), 16);
        var b = Convert.ToByte(hex.Substring(4, 2), 16);
        var a = hex.Length == 8
            ? Convert.ToByte(hex.Substring(6, 2), 16)
            : (Byte)255;

        return $"rgba({r}, {g}, {b}, {a / 255d:0.###})";
    }

    private static String? RgbaToHex(String? rgba)
    {
        if (rgba.HasNothing())
            return null;

        var parts = rgba
            .Replace("rgba(", "", StringComparison.OrdinalIgnoreCase)
            .Replace("rgb(", "", StringComparison.OrdinalIgnoreCase)
            .Replace(")", "")
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (parts.Length < 3)
            return null;

        var r = Byte.Parse(parts[0], CultureInfo.InvariantCulture);
        var g = Byte.Parse(parts[1], CultureInfo.InvariantCulture);
        var b = Byte.Parse(parts[2], CultureInfo.InvariantCulture);
        var aFloat = parts.Length > 3 ? Double.Parse(parts[3], CultureInfo.InvariantCulture) : 1d;
        var a = (Byte)Math.Round(aFloat * 255);

        return $"#{r:x2}{g:x2}{b:x2}{a:x2}";
    }
}