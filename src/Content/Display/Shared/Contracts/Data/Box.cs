namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class Box : Observable, IValidates
{
    public enum CustomFonts
    {
        Default,
        CustomFont1,
        CustomFont2,
        CustomFont3,
        CustomFont4,
    }

    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? BackgroundColor
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ImageFile BackgroundImageFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public String? BorderColor
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? BorderRadius
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? BorderThickness
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? BorderThicknessTop
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? BorderThicknessLeft
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? BorderThicknessRight
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? BorderThicknessBottom
    {
        get;
        set => SetProperty(ref field, value);
    }

    public CustomFonts CustomFontIndex
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? FontSize
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? FontWeight
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ForegroundColor
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? MarginBottom
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? MarginLeft
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? MarginRight
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? MarginTop
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? PaddingBottom
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? PaddingLeft
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? PaddingRight
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? PaddingTop
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ShadowBlurRadius
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ShadowColor
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ShadowOffsetX
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ShadowOffsetY
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ShadowSpreadRadius
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? TextShadowBlurRadius
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? TextShadowColor
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? TextShadowOffsetX
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? TextShadowOffsetY
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? HeadingLineHeight
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ParagraphLineHeight
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            BorderColor.ValidateColorProperty(nameof(BorderColor), errors);
            BorderRadius.ValidateWidthProperty(nameof(BorderRadius), errors);
            BorderThickness.ValidateWidthProperty(nameof(BorderThickness), errors);
            BorderThicknessTop.ValidateWidthProperty(nameof(BorderThicknessTop), errors);
            BorderThicknessLeft.ValidateWidthProperty(nameof(BorderThicknessLeft), errors);
            BorderThicknessRight.ValidateWidthProperty(nameof(BorderThicknessRight), errors);
            BorderThicknessBottom.ValidateWidthProperty(nameof(BorderThicknessBottom), errors);
            FontSize.ValidateWidthProperty(nameof(FontSize), errors);
            FontWeight.ValidateWeightProperty(nameof(FontWeight), errors);
            BackgroundColor.ValidateColorProperty(nameof(BackgroundColor), errors);
            ForegroundColor.ValidateColorProperty(nameof(ForegroundColor), errors);
            TextShadowBlurRadius.ValidateWidthProperty(nameof(TextShadowBlurRadius), errors);
            TextShadowColor.ValidateColorProperty(nameof(TextShadowColor), errors);
            TextShadowOffsetX.ValidateWidthProperty(nameof(TextShadowOffsetX), errors);
            TextShadowOffsetY.ValidateWidthProperty(nameof(TextShadowOffsetY), errors);
            MarginBottom.ValidateWidthProperty(nameof(MarginBottom), errors);
            MarginLeft.ValidateWidthProperty(nameof(MarginLeft), errors);
            MarginRight.ValidateWidthProperty(nameof(MarginRight), errors);
            MarginTop.ValidateWidthProperty(nameof(MarginTop), errors);
            PaddingBottom.ValidateWidthProperty(nameof(PaddingBottom), errors);
            PaddingLeft.ValidateWidthProperty(nameof(PaddingLeft), errors);
            PaddingRight.ValidateWidthProperty(nameof(PaddingRight), errors);
            PaddingTop.ValidateWidthProperty(nameof(PaddingTop), errors);
            ShadowBlurRadius.ValidateWidthProperty(nameof(ShadowBlurRadius), errors);
            ShadowColor.ValidateColorProperty(nameof(ShadowColor), errors);
            ShadowOffsetX.ValidateWidthProperty(nameof(ShadowOffsetX), errors);
            ShadowOffsetY.ValidateWidthProperty(nameof(ShadowOffsetY), errors);
            ShadowSpreadRadius.ValidateWidthProperty(nameof(ShadowSpreadRadius), errors);
            HeadingLineHeight.ValidateUnitProperty(nameof(HeadingLineHeight), errors);
            ParagraphLineHeight.ValidateUnitProperty(nameof(ParagraphLineHeight), errors);
        });
    }
}