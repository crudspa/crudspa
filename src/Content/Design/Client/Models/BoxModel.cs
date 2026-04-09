namespace Crudspa.Content.Design.Client.Models;

public class BoxModel : ModalModel
{
    private void HandleBoxChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(Box));
    public void SetActiveTab(String key) => ActiveTab = key;

    private Box _box;

    public BoxModel(IScrollService scrollService, Box box) : base(scrollService)
    {
        _box = box;
        _box.PropertyChanged += HandleBoxChanged;
    }

    public override void Dispose()
    {
        _box.PropertyChanged -= HandleBoxChanged;
        base.Dispose();
    }

    public override Task Show()
    {
        BorderColorState = _box.BorderColor.HasSomething() ? OverrideState.Custom : OverrideState.Default;
        BorderRadiusState = _box.BorderRadius.HasSomething() ? OverrideState.Custom : OverrideState.Default;
        BorderThicknessState = _box.BorderThickness.HasSomething() ? OverrideState.Custom : OverrideState.Default;
        BorderThicknessTopState = _box.BorderThicknessTop.HasSomething() ? OverrideState.Custom : OverrideState.Default;
        BorderThicknessLeftState = _box.BorderThicknessLeft.HasSomething() ? OverrideState.Custom : OverrideState.Default;
        BorderThicknessRightState = _box.BorderThicknessRight.HasSomething() ? OverrideState.Custom : OverrideState.Default;
        BorderThicknessBottomState = _box.BorderThicknessBottom.HasSomething() ? OverrideState.Custom : OverrideState.Default;
        FontSizeState = _box.FontSize.HasSomething() ? OverrideState.Custom : OverrideState.Default;
        FontWeightState = _box.FontWeight.HasSomething() ? OverrideState.Custom : OverrideState.Default;
        BackgroundColorState = _box.BackgroundColor.HasSomething() ? OverrideState.Custom : OverrideState.Default;
        ForegroundColorState = _box.ForegroundColor.HasSomething() ? OverrideState.Custom : OverrideState.Default;
        TextShadowBlurRadiusState = _box.TextShadowBlurRadius.HasSomething() ? OverrideState.Custom : OverrideState.Default;
        TextShadowColorState = _box.TextShadowColor.HasSomething() ? OverrideState.Custom : OverrideState.Default;
        TextShadowOffsetXState = _box.TextShadowOffsetX.HasSomething() ? OverrideState.Custom : OverrideState.Default;
        TextShadowOffsetYState = _box.TextShadowOffsetY.HasSomething() ? OverrideState.Custom : OverrideState.Default;
        MarginBottomState = _box.MarginBottom.HasSomething() ? OverrideState.Custom : OverrideState.Default;
        MarginLeftState = _box.MarginLeft.HasSomething() ? OverrideState.Custom : OverrideState.Default;
        MarginRightState = _box.MarginRight.HasSomething() ? OverrideState.Custom : OverrideState.Default;
        MarginTopState = _box.MarginTop.HasSomething() ? OverrideState.Custom : OverrideState.Default;
        PaddingBottomState = _box.PaddingBottom.HasSomething() ? OverrideState.Custom : OverrideState.Default;
        PaddingLeftState = _box.PaddingLeft.HasSomething() ? OverrideState.Custom : OverrideState.Default;
        PaddingRightState = _box.PaddingRight.HasSomething() ? OverrideState.Custom : OverrideState.Default;
        PaddingTopState = _box.PaddingTop.HasSomething() ? OverrideState.Custom : OverrideState.Default;
        ShadowBlurRadiusState = _box.ShadowBlurRadius.HasSomething() ? OverrideState.Custom : OverrideState.Default;
        ShadowColorState = _box.ShadowColor.HasSomething() ? OverrideState.Custom : OverrideState.Default;
        ShadowOffsetXState = _box.ShadowOffsetX.HasSomething() ? OverrideState.Custom : OverrideState.Default;
        ShadowOffsetYState = _box.ShadowOffsetY.HasSomething() ? OverrideState.Custom : OverrideState.Default;
        ShadowSpreadRadiusState = _box.ShadowSpreadRadius.HasSomething() ? OverrideState.Custom : OverrideState.Default;
        HeadingLineHeightState = _box.HeadingLineHeight.HasSomething() ? OverrideState.Custom : OverrideState.Default;
        ParagraphLineHeightState = _box.ParagraphLineHeight.HasSomething() ? OverrideState.Custom : OverrideState.Default;

        return base.Show();
    }

    public override Task Hide()
    {
        if (BorderColorState == OverrideState.Default || _box.BorderColor.HasNothing())
            _box.BorderColor = null;

        if (BorderRadiusState == OverrideState.Default || _box.BorderRadius.HasNothing())
            _box.BorderRadius = null;

        if (BorderThicknessState == OverrideState.Default || _box.BorderThickness.HasNothing())
            _box.BorderThickness = null;

        if (BorderThicknessTopState == OverrideState.Default || _box.BorderThicknessTop.HasNothing())
            _box.BorderThicknessTop = null;

        if (BorderThicknessLeftState == OverrideState.Default || _box.BorderThicknessLeft.HasNothing())
            _box.BorderThicknessLeft = null;

        if (BorderThicknessRightState == OverrideState.Default || _box.BorderThicknessRight.HasNothing())
            _box.BorderThicknessRight = null;

        if (BorderThicknessBottomState == OverrideState.Default || _box.BorderThicknessBottom.HasNothing())
            _box.BorderThicknessBottom = null;

        if (FontSizeState == OverrideState.Default || _box.FontSize.HasNothing())
            _box.FontSize = null;

        if (FontWeightState == OverrideState.Default || _box.FontWeight.HasNothing())
            _box.FontWeight = null;

        if (BackgroundColorState == OverrideState.Default || _box.BackgroundColor.HasNothing())
            _box.BackgroundColor = null;

        if (ForegroundColorState == OverrideState.Default || _box.ForegroundColor.HasNothing())
            _box.ForegroundColor = null;

        if (TextShadowBlurRadiusState == OverrideState.Default || _box.TextShadowBlurRadius.HasNothing())
            _box.TextShadowBlurRadius = null;

        if (TextShadowColorState == OverrideState.Default || _box.TextShadowColor.HasNothing())
            _box.TextShadowColor = null;

        if (TextShadowOffsetXState == OverrideState.Default || _box.TextShadowOffsetX.HasNothing())
            _box.TextShadowOffsetX = null;

        if (TextShadowOffsetYState == OverrideState.Default || _box.TextShadowOffsetY.HasNothing())
            _box.TextShadowOffsetY = null;

        if (MarginBottomState == OverrideState.Default || _box.MarginBottom.HasNothing())
            _box.MarginBottom = null;

        if (MarginLeftState == OverrideState.Default || _box.MarginLeft.HasNothing())
            _box.MarginLeft = null;

        if (MarginRightState == OverrideState.Default || _box.MarginRight.HasNothing())
            _box.MarginRight = null;

        if (MarginTopState == OverrideState.Default || _box.MarginTop.HasNothing())
            _box.MarginTop = null;

        if (PaddingBottomState == OverrideState.Default || _box.PaddingBottom.HasNothing())
            _box.PaddingBottom = null;

        if (PaddingLeftState == OverrideState.Default || _box.PaddingLeft.HasNothing())
            _box.PaddingLeft = null;

        if (PaddingRightState == OverrideState.Default || _box.PaddingRight.HasNothing())
            _box.PaddingRight = null;

        if (PaddingTopState == OverrideState.Default || _box.PaddingTop.HasNothing())
            _box.PaddingTop = null;

        if (ShadowBlurRadiusState == OverrideState.Default || _box.ShadowBlurRadius.HasNothing())
            _box.ShadowBlurRadius = null;

        if (ShadowColorState == OverrideState.Default || _box.ShadowColor.HasNothing())
            _box.ShadowColor = null;

        if (ShadowOffsetXState == OverrideState.Default || _box.ShadowOffsetX.HasNothing())
            _box.ShadowOffsetX = null;

        if (ShadowOffsetYState == OverrideState.Default || _box.ShadowOffsetY.HasNothing())
            _box.ShadowOffsetY = null;

        if (ShadowSpreadRadiusState == OverrideState.Default || _box.ShadowSpreadRadius.HasNothing())
            _box.ShadowSpreadRadius = null;

        if (HeadingLineHeightState == OverrideState.Default || _box.HeadingLineHeight.HasNothing())
            _box.HeadingLineHeight = null;

        if (ParagraphLineHeightState == OverrideState.Default || _box.ParagraphLineHeight.HasNothing())
            _box.ParagraphLineHeight = null;

        return base.Hide();
    }

    public Box Box
    {
        get => _box;
        set => SetProperty(ref _box, value);
    }

    public String ActiveTab
    {
        get;
        set => SetProperty(ref field, value);
    } = "Border";

    public OverrideState BorderColorState
    {
        get;
        set => SetProperty(ref field, value);
    }

    public OverrideState BorderRadiusState
    {
        get;
        set => SetProperty(ref field, value);
    }

    public OverrideState BorderThicknessState
    {
        get;
        set => SetProperty(ref field, value);
    }

    public OverrideState BorderThicknessTopState
    {
        get;
        set => SetProperty(ref field, value);
    }

    public OverrideState BorderThicknessLeftState
    {
        get;
        set => SetProperty(ref field, value);
    }

    public OverrideState BorderThicknessRightState
    {
        get;
        set => SetProperty(ref field, value);
    }

    public OverrideState BorderThicknessBottomState
    {
        get;
        set => SetProperty(ref field, value);
    }

    public OverrideState FontSizeState
    {
        get;
        set => SetProperty(ref field, value);
    }

    public OverrideState FontWeightState
    {
        get;
        set => SetProperty(ref field, value);
    }

    public OverrideState BackgroundColorState
    {
        get;
        set => SetProperty(ref field, value);
    }

    public OverrideState ForegroundColorState
    {
        get;
        set => SetProperty(ref field, value);
    }

    public OverrideState TextShadowBlurRadiusState
    {
        get;
        set => SetProperty(ref field, value);
    }

    public OverrideState TextShadowColorState
    {
        get;
        set => SetProperty(ref field, value);
    }

    public OverrideState TextShadowOffsetXState
    {
        get;
        set => SetProperty(ref field, value);
    }

    public OverrideState TextShadowOffsetYState
    {
        get;
        set => SetProperty(ref field, value);
    }

    public OverrideState MarginBottomState
    {
        get;
        set => SetProperty(ref field, value);
    }

    public OverrideState MarginLeftState
    {
        get;
        set => SetProperty(ref field, value);
    }

    public OverrideState MarginRightState
    {
        get;
        set => SetProperty(ref field, value);
    }

    public OverrideState MarginTopState
    {
        get;
        set => SetProperty(ref field, value);
    }

    public OverrideState PaddingBottomState
    {
        get;
        set => SetProperty(ref field, value);
    }

    public OverrideState PaddingLeftState
    {
        get;
        set => SetProperty(ref field, value);
    }

    public OverrideState PaddingRightState
    {
        get;
        set => SetProperty(ref field, value);
    }

    public OverrideState PaddingTopState
    {
        get;
        set => SetProperty(ref field, value);
    }

    public OverrideState ShadowBlurRadiusState
    {
        get;
        set => SetProperty(ref field, value);
    }

    public OverrideState ShadowColorState
    {
        get;
        set => SetProperty(ref field, value);
    }

    public OverrideState ShadowOffsetXState
    {
        get;
        set => SetProperty(ref field, value);
    }

    public OverrideState ShadowOffsetYState
    {
        get;
        set => SetProperty(ref field, value);
    }

    public OverrideState ShadowSpreadRadiusState
    {
        get;
        set => SetProperty(ref field, value);
    }

    public OverrideState HeadingLineHeightState
    {
        get;
        set => SetProperty(ref field, value);
    }

    public OverrideState ParagraphLineHeightState
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String Description
    {
        get
        {
            List<String> values = [];
            if (Box.BorderColor.HasSomething())
                values.Add($"{nameof(Box.BorderColor).InsertSpaces()}: {Box.BorderColor}");

            if (Box.BorderRadius.HasSomething())
                values.Add($"{nameof(Box.BorderRadius).InsertSpaces()}: {Box.BorderRadius}");

            if (Box.BorderThickness.HasSomething())
                values.Add($"{nameof(Box.BorderThickness).InsertSpaces()}: {Box.BorderThickness}");

            if (Box.BorderThicknessTop.HasSomething())
                values.Add($"{nameof(Box.BorderThicknessTop).InsertSpaces()}: {Box.BorderThicknessTop}");

            if (Box.BorderThicknessLeft.HasSomething())
                values.Add($"{nameof(Box.BorderThicknessLeft).InsertSpaces()}: {Box.BorderThicknessLeft}");

            if (Box.BorderThicknessRight.HasSomething())
                values.Add($"{nameof(Box.BorderThicknessRight).InsertSpaces()}: {Box.BorderThicknessRight}");

            if (Box.BorderThicknessBottom.HasSomething())
                values.Add($"{nameof(Box.BorderThicknessBottom).InsertSpaces()}: {Box.BorderThicknessBottom}");

            if (Box.CustomFontIndex != Box.CustomFonts.Default)
                values.Add($"Font: {Box.CustomFontIndex.GetLabel()}");

            if (Box.FontSize.HasSomething())
                values.Add($"{nameof(Box.FontSize).InsertSpaces()}: {Box.FontSize}");

            if (Box.FontWeight.HasSomething())
                values.Add($"{nameof(Box.FontWeight).InsertSpaces()}: {Box.FontWeight}");

            if (Box.BackgroundImageFile.Name.HasSomething())
                values.Add("Background Image: [image]");

            if (Box.BackgroundColor.HasSomething())
                values.Add($"{nameof(Box.BackgroundColor).InsertSpaces()}: {Box.BackgroundColor}");

            if (Box.ForegroundColor.HasSomething())
                values.Add($"{nameof(Box.ForegroundColor).InsertSpaces()}: {Box.ForegroundColor}");

            if (Box.MarginTop.HasSomething())
                values.Add($"{nameof(Box.MarginTop).InsertSpaces()}: {Box.MarginTop}");

            if (Box.MarginLeft.HasSomething())
                values.Add($"{nameof(Box.MarginLeft).InsertSpaces()}: {Box.MarginLeft}");

            if (Box.MarginRight.HasSomething())
                values.Add($"{nameof(Box.MarginRight).InsertSpaces()}: {Box.MarginRight}");

            if (Box.MarginBottom.HasSomething())
                values.Add($"{nameof(Box.MarginBottom).InsertSpaces()}: {Box.MarginBottom}");

            if (Box.PaddingTop.HasSomething())
                values.Add($"{nameof(Box.PaddingTop).InsertSpaces()}: {Box.PaddingTop}");

            if (Box.PaddingLeft.HasSomething())
                values.Add($"{nameof(Box.PaddingLeft).InsertSpaces()}: {Box.PaddingLeft}");

            if (Box.PaddingRight.HasSomething())
                values.Add($"{nameof(Box.PaddingRight).InsertSpaces()}: {Box.PaddingRight}");

            if (Box.PaddingBottom.HasSomething())
                values.Add($"{nameof(Box.PaddingBottom).InsertSpaces()}: {Box.PaddingBottom}");

            if (Box.ShadowBlurRadius.HasSomething())
                values.Add($"{nameof(Box.ShadowBlurRadius).InsertSpaces()}: {Box.ShadowBlurRadius}");

            if (Box.ShadowColor.HasSomething())
                values.Add($"{nameof(Box.ShadowColor).InsertSpaces()}: {Box.ShadowColor}");

            if (Box.ShadowOffsetX.HasSomething())
                values.Add($"{nameof(Box.ShadowOffsetX).InsertSpaces()}: {Box.ShadowOffsetX}");

            if (Box.ShadowOffsetY.HasSomething())
                values.Add($"{nameof(Box.ShadowOffsetY).InsertSpaces()}: {Box.ShadowOffsetY}");

            if (Box.ShadowSpreadRadius.HasSomething())
                values.Add($"{nameof(Box.ShadowSpreadRadius).InsertSpaces()}: {Box.ShadowSpreadRadius}");

            if (Box.HeadingLineHeight.HasSomething())
                values.Add($"{nameof(Box.HeadingLineHeight).InsertSpaces()}: {Box.HeadingLineHeight}");

            if (Box.ParagraphLineHeight.HasSomething())
                values.Add($"{nameof(Box.ParagraphLineHeight).InsertSpaces()}: {Box.ParagraphLineHeight}");

            if (Box.TextShadowBlurRadius.HasSomething())
                values.Add($"{nameof(Box.TextShadowBlurRadius).InsertSpaces()}: {Box.TextShadowBlurRadius}");

            if (Box.TextShadowColor.HasSomething())
                values.Add($"{nameof(Box.TextShadowColor).InsertSpaces()}: {Box.TextShadowColor}");

            if (Box.TextShadowOffsetX.HasSomething())
                values.Add($"{nameof(Box.TextShadowOffsetX).InsertSpaces()}: {Box.TextShadowOffsetX}");

            if (Box.TextShadowOffsetY.HasSomething())
                values.Add($"{nameof(Box.TextShadowOffsetY).InsertSpaces()}: {Box.TextShadowOffsetY}");

            return values.HasItems() ? String.Join(" | ", values) : "None";
        }
    }
}