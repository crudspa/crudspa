using Crudspa.Framework.Core.Shared.Contracts.Config.RuleType;
using System.Text;

namespace Crudspa.Content.Display.Server.Extensions;

public static class PortalStylesEx
{
    extension(PortalStyles portalStyles)
    {
        public String ToScssVariables()
        {
            var scss = String.Empty;

            foreach (var style in portalStyles.Styles.OrderBy(x => x.Rule.Name))
            {
                switch (style.Rule.RuleType!.Id)
                {
                    case var id when id == RuleTypeIds.Color:
                        var colorConfig = style.ConfigJson.FromJson<ColorConfig>();
                        if (colorConfig is not null)
                            scss += $"${style.Rule.Key}Color: {colorConfig.Color};{Environment.NewLine}";
                        break;

                    case var id when id == RuleTypeIds.ColorPair:
                        var colorPairConfig = style.ConfigJson.FromJson<ColorPairConfig>();
                        if (colorPairConfig is not null)
                            scss += $"${style.Rule.Key}BackgroundColor: {colorPairConfig.Background};{Environment.NewLine}${style.Rule.Key}ForegroundColor: {colorPairConfig.Foreground};{Environment.NewLine}";
                        break;

                    case var id when id == RuleTypeIds.Font:
                        var fontConfig = style.ConfigJson.FromJson<FontConfig>();
                        if (fontConfig is not null)
                        {
                            if (fontConfig.Id.HasSomething())
                            {
                                var font = portalStyles.Fonts.FirstOrDefault(x => x.Id.Equals(fontConfig.Id));
                                if (font is not null)
                                    scss += $"${style.Rule.Key}FontFamily: '{font.Name}', system-ui, sans-serif;{Environment.NewLine}";
                            }

                            scss += $"${style.Rule.Key}FontSize: {fontConfig.Size};{Environment.NewLine}${style.Rule.Key}FontWeight: {fontConfig.Weight};{Environment.NewLine}";
                        }

                        break;

                    case var id when id == RuleTypeIds.Margin:
                        var marginConfig = style.ConfigJson.FromJson<MarginConfig>();
                        if (marginConfig is not null)
                            scss += $"${style.Rule.Key}Margin: {marginConfig.Top} {marginConfig.Right} {marginConfig.Bottom} {marginConfig.Left};{Environment.NewLine}";
                        break;

                    case var id when id == RuleTypeIds.Padding:
                        var paddingConfig = style.ConfigJson.FromJson<PaddingConfig>();
                        if (paddingConfig is not null)
                            scss += $"${style.Rule.Key}Padding: {paddingConfig.Top} {paddingConfig.Right} {paddingConfig.Bottom} {paddingConfig.Left};{Environment.NewLine}";
                        break;

                    case var id when id == RuleTypeIds.Roundness:
                        var roundnessConfig = style.ConfigJson.FromJson<RoundnessConfig>();
                        if (roundnessConfig is not null)
                            scss += $"${style.Rule.Key}BorderRadius: {roundnessConfig.Radius};{Environment.NewLine}";
                        break;

                    case var id when id == RuleTypeIds.Zoom:
                        var zoomConfig = style.ConfigJson.FromJson<ZoomConfig>();
                        if (zoomConfig is not null)
                        {
                            switch (zoomConfig.Level)
                            {
                                case ZoomConfig.Levels.Smaller:
                                    scss +=
                                        @"$htmlMinFontSize: .65em;
$htmlTinyFontSize: .7em;
$htmlSmallFontSize: .75em;
$htmlMediumFontSize: .8em;
$htmlLargeFontSize: .85em;
$htmlWideFontSize: .9em;
$htmlMaxFontSize: .85em;
";
                                    break;

                                case ZoomConfig.Levels.Small:
                                    scss +=
                                        @"$htmlMinFontSize: .6em;
$htmlTinyFontSize: .65em;
$htmlSmallFontSize: .7em;
$htmlMediumFontSize: .75em;
$htmlLargeFontSize: .8em;
$htmlWideFontSize: .85em;
$htmlMaxFontSize: .9em;
";
                                    break;

                                case ZoomConfig.Levels.Regular:
                                    scss +=
                                        @"$htmlMinFontSize: .7em;
$htmlTinyFontSize: .7em;
$htmlSmallFontSize: .75em;
$htmlMediumFontSize: .8em;
$htmlLargeFontSize: .85em;
$htmlWideFontSize: .9em;
$htmlMaxFontSize: .95em;
";
                                    break;

                                case ZoomConfig.Levels.Large:
                                    scss +=
                                        @"$htmlMinFontSize: .7em;
$htmlTinyFontSize: .75em;
$htmlSmallFontSize: .8em;
$htmlMediumFontSize: .85em;
$htmlLargeFontSize: .9em;
$htmlWideFontSize: .95em;
$htmlMaxFontSize: 1em;
";
                                    break;

                                case ZoomConfig.Levels.Larger:
                                    scss +=
                                        @"$htmlMinFontSize: .75em;
$htmlTinyFontSize: .8em;
$htmlSmallFontSize: .85em;
$htmlMediumFontSize: .9em;
$htmlLargeFontSize: .95em;
$htmlWideFontSize: 1em;
$htmlMaxFontSize: 1.05em;
";
                                    break;
                            }
                        }

                        break;
                }
            }

            return scss;
        }

        public String ToFontFaceCss()
        {
            var css = new StringBuilder();

            foreach (var font in portalStyles.Fonts
                         .Where(x => x.FileFile.Id.HasValue && x.Name.HasSomething())
                         .OrderBy(x => x.Name))
            {
                var fontUrl = font.FileFile.FetchUrl();
                if (fontUrl.HasNothing())
                    continue;

                css.Append("@font-face{font-family:'");
                css.Append(font.Name!.Replace("'", "\\'"));
                css.Append("';src:url('");
                css.Append(fontUrl);
                css.Append("')");

                var fontFormat = ToCssFontFormat(font.FileFile.Format);
                if (fontFormat.HasSomething())
                {
                    css.Append(" format('");
                    css.Append(fontFormat);
                    css.Append("')");
                }

                css.Append(";}");
                css.Append(Environment.NewLine);
            }

            return css.ToString();
        }
    }

    private static String? ToCssFontFormat(String? fontFormat)
    {
        return fontFormat?.ToLowerInvariant() switch
        {
            ".otf" => "opentype",
            ".ttf" => "truetype",
            ".woff" => "woff",
            ".woff2" => "woff2",
            _ => null,
        };
    }
}