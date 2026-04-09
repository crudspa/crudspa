using System.Text.RegularExpressions;

namespace Crudspa.Framework.Core.Shared.Extensions;

public static partial class CssEx
{
    private static readonly String[] DefaultCssWidthUnits = ["em", "px"];
    private static readonly String[] ResponsiveCssLengthUnits = ["em", "px", "rem", "vw", "vh", "vmin", "vmax", "ch", "ex"];

    [GeneratedRegex(@"^#[0-9A-Fa-f]{3}([0-9A-Fa-f]{3})?([0-9A-Fa-f]{2})?$")]
    private static partial Regex CssColorRegex();

    private static Boolean IsValidCssLength(String? input, params String[] units)
    {
        if (input.HasNothing() || input.Length > 10)
            return false;

        var matchingUnit = units
            .OrderByDescending(x => x.Length)
            .FirstOrDefault(unit => input.EndsWith(unit, StringComparison.OrdinalIgnoreCase));

        if (matchingUnit.HasNothing())
            return false;

        var numberPart = input.Substring(0, input.Length - matchingUnit.Length);

        return Decimal.TryParse(numberPart, NumberStyles.Float, CultureInfo.InvariantCulture, out _);
    }

    extension(String? input)
    {
        public Boolean IsValidCssColor()
        {
            if (input.HasNothing() || input.Length > 10)
                return false;

            var hexColorRegex = CssColorRegex();
            return hexColorRegex.IsMatch(input);
        }

        public Boolean IsValidCssWeight()
        {
            if (input.HasNothing() || input.Length > 10)
                return false;

            return input.Equals("100")
                || input.Equals("200")
                || input.Equals("300")
                || input.Equals("400")
                || input.Equals("500")
                || input.Equals("600")
                || input.Equals("700")
                || input.Equals("800")
                || input.Equals("900");
        }

        public Boolean IsValidCssWidth()
        {
            return IsValidCssLength(input, DefaultCssWidthUnits);
        }

        public Boolean IsValidCssResponsiveLength()
        {
            return IsValidCssLength(input, ResponsiveCssLengthUnits);
        }

        public Boolean IsValidCssPercentage()
        {
            if (input.HasNothing() || input.Length > 10)
                return false;

            if (!input.EndsWith("%"))
                return false;

            var numberPart = input.Substring(0, input.Length - 1);

            return Decimal.TryParse(numberPart, NumberStyles.Float, CultureInfo.InvariantCulture, out _);
        }

        public Boolean IsValidCssLengthOrPercentage()
        {
            return input.IsValidCssResponsiveLength() || input.IsValidCssPercentage();
        }

        public Boolean IsValidCssResponsiveWidth()
        {
            if (input.HasNothing() || input.Length > 10)
                return false;

            return input.IsBasically("auto") || input.IsValidCssLengthOrPercentage();
        }

        public Boolean IsValidCssPositiveNumber()
        {
            if (input.HasNothing() || input.Length > 10)
                return false;

            var parsed = Decimal.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out var number);

            if (!parsed)
                return false;

            return number >= 0;
        }
    }
}