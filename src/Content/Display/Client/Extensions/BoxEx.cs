using System.Text;

namespace Crudspa.Content.Display.Client.Extensions;

public static class BoxEx
{
    extension(Box box)
    {
        public String BoxClasses()
        {
            var classes = new StringBuilder(String.Empty);

            if (box.CustomFontIndex == Box.CustomFonts.CustomFont1)
                classes.Append("c-custom-font1 ");
            else if (box.CustomFontIndex == Box.CustomFonts.CustomFont2)
                classes.Append("c-custom-font2 ");
            else if (box.CustomFontIndex == Box.CustomFonts.CustomFont3)
                classes.Append("c-custom-font3 ");
            else if (box.CustomFontIndex == Box.CustomFonts.CustomFont4)
                classes.Append("c-custom-font4 ");

            if (box.ForegroundColor.HasSomething() || box.HeadingLineHeight.HasSomething() || box.ParagraphLineHeight.HasSomething())
                classes.Append("c-box-text ");

            return classes.ToString();
        }

        public String BoxStyles()
        {
            var styles = new StringBuilder(String.Empty);

            if (box.BorderColor.HasSomething() || HasBorderThickness(box))
                styles.Append("border-style: solid; ");

            AppendBorderColorStyles(styles, box);

            if (box.BorderRadius.HasSomething())
                styles.Append($"border-radius: {box.BorderRadius}; ");

            AppendBorderThicknessStyles(styles, box);

            if (box.BackgroundColor.HasSomething())
                styles.Append($"background-color: {box.BackgroundColor}; ");

            if (box.BackgroundImageFile.Id.HasSomething())
                styles.Append($"background-size: cover; background-position: center; background-repeat: no-repeat; background-image: url('{box.BackgroundImageFile.FetchUrl()}'); ");

            if (box.FontSize.HasSomething())
                styles.Append($"font-size: {box.FontSize}; ");

            if (box.FontWeight.HasSomething())
                styles.Append($"font-weight: {box.FontWeight}; ");

            if (box.ForegroundColor.HasSomething())
            {
                styles.Append($"--c-box-foreground-color: {box.ForegroundColor}; ");
                styles.Append($"color: {box.ForegroundColor}; ");
            }

            if (box.HeadingLineHeight.HasSomething())
                styles.Append($"--c-box-heading-line-height: {box.HeadingLineHeight}; ");

            if (box.ParagraphLineHeight.HasSomething())
                styles.Append($"--c-box-paragraph-line-height: {box.ParagraphLineHeight}; ");

            if (box.MarginBottom.HasSomething())
                styles.Append($"margin-bottom: {box.MarginBottom}; ");

            if (box.MarginLeft.HasSomething())
                styles.Append($"margin-left: {box.MarginLeft}; ");

            if (box.MarginRight.HasSomething())
                styles.Append($"margin-right: {box.MarginRight}; ");

            if (box.MarginTop.HasSomething())
                styles.Append($"margin-top: {box.MarginTop}; ");

            if (box.PaddingBottom.HasSomething())
                styles.Append($"padding-bottom: {box.PaddingBottom}; ");

            if (box.PaddingLeft.HasSomething())
                styles.Append($"padding-left: {box.PaddingLeft}; ");

            if (box.PaddingRight.HasSomething())
                styles.Append($"padding-right: {box.PaddingRight}; ");

            if (box.PaddingTop.HasSomething())
                styles.Append($"padding-top: {box.PaddingTop}; ");

            if (box.ShadowBlurRadius.HasSomething() || box.ShadowColor.HasSomething() ||
                box.ShadowOffsetX.HasSomething() || box.ShadowOffsetY.HasSomething())
                styles.Append($"box-shadow: {box.ShadowOffsetX} {box.ShadowOffsetY} {box.ShadowBlurRadius} {box.ShadowSpreadRadius} {box.ShadowColor}; ");

            if (box.TextShadowBlurRadius.HasSomething() || box.TextShadowColor.HasSomething() ||
                box.TextShadowOffsetX.HasSomething() || box.TextShadowOffsetY.HasSomething())
                styles.Append($"text-shadow: {box.TextShadowOffsetX} {box.TextShadowOffsetY} {box.TextShadowBlurRadius} {box.TextShadowColor}; ");

            return styles.ToString();
        }
    }

    private static Boolean HasBorderThickness(Box box)
    {
        return box.BorderThickness.HasSomething()
            || box.BorderThicknessTop.HasSomething()
            || box.BorderThicknessLeft.HasSomething()
            || box.BorderThicknessRight.HasSomething()
            || box.BorderThicknessBottom.HasSomething();
    }

    private static void AppendBorderColorStyles(StringBuilder styles, Box box)
    {
        if (box.BorderColor.HasSomething())
            styles.Append($"border-color: {box.BorderColor}; ");
        else if (HasBorderThickness(box))
            styles.Append("border-color: var(--default-border-color); ");
    }

    private static void AppendBorderThicknessStyles(StringBuilder styles, Box box)
    {
        if (!HasBorderThickness(box))
            return;

        styles.Append($"border-width: {box.BorderThickness ?? "0"}; ");

        if (box.BorderThicknessTop.HasSomething())
            styles.Append($"border-top-width: {box.BorderThicknessTop}; ");

        if (box.BorderThicknessLeft.HasSomething())
            styles.Append($"border-left-width: {box.BorderThicknessLeft}; ");

        if (box.BorderThicknessRight.HasSomething())
            styles.Append($"border-right-width: {box.BorderThicknessRight}; ");

        if (box.BorderThicknessBottom.HasSomething())
            styles.Append($"border-bottom-width: {box.BorderThicknessBottom}; ");
    }
}