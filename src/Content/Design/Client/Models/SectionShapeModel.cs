using DataContainer = Crudspa.Content.Display.Shared.Contracts.Data.Container;

namespace Crudspa.Content.Design.Client.Models;

public class SectionShapeModel : ModalModel
{
    private readonly SectionEditModel _sectionEditModel;

    public SectionShapeModel(IScrollService scrollService, SectionEditModel sectionEditModel)
        : base(scrollService)
    {
        _sectionEditModel = sectionEditModel;
        Shapes = SectionShapes.All();
        SelectedShapeId = Shapes.FirstOrDefault()?.Id;
    }

    public List<SectionShapeChoice> Shapes { get; }

    public Guid? SelectedShapeId
    {
        get;
        set
        {
            if (!SetProperty(ref field, value))
                return;

            RaisePropertyChanged(nameof(SelectedShape));
            RaisePropertyChanged(nameof(PreviewSection));
            RaisePropertyChanged(nameof(ExistingContentMessage));
        }
    }

    public SectionShapeChoice? SelectedShape => Shapes.FirstOrDefault(x => x.Id == SelectedShapeId);

    public Boolean? Shadowed
    {
        get;
        set
        {
            if (!SetProperty(ref field, value))
                return;

            RaisePropertyChanged(nameof(PreviewSection));
        }
    }

    public Boolean? Bordered
    {
        get;
        set
        {
            if (!SetProperty(ref field, value))
                return;

            RaisePropertyChanged(nameof(PreviewSection));
        }
    }

    public Boolean? Rounded
    {
        get;
        set
        {
            if (!SetProperty(ref field, value))
                return;

            RaisePropertyChanged(nameof(PreviewSection));
        }
    }

    public Section? PreviewSection => SelectedShape?.BuildSection(bordered: Bordered == true, rounded: Rounded == true, shadowed: Shadowed == true);

    public Boolean HasExistingContent => _sectionEditModel.ElementEditBatchModel.Entities.HasItems();

    public String? ExistingContentMessage => HasExistingContent && SelectedShape?.ReplacesContent == true
        ? "Applying this shape will replace the elements that are already in this section."
        : null;

    public override Task Show()
    {
        Alerts.Clear();
        SelectedShapeId ??= Shapes.FirstOrDefault()?.Id;
        return base.Show();
    }

    public async Task Apply()
    {
        var selectedShape = SelectedShape;
        var previewSection = PreviewSection;

        if (selectedShape is null || previewSection is null)
        {
            Alerts.Add(new()
            {
                Type = Alert.AlertType.Error,
                Errors = [new() { Message = "Choose a section shape before continuing." }],
            });
            return;
        }

        var response = await WithWaiting(
            selectedShape.ReplacesContent ? "Applying shape..." : "Closing...",
            () => ApplyShape(previewSection, selectedShape.ReplacesContent));

        if (response.Ok)
            await Hide();
    }

    private async Task<Response> ApplyShape(Section shapeSection, Boolean replacesContent)
    {
        if (_sectionEditModel.Entity is null)
            return new("The section is not ready yet.");

        if (!replacesContent)
            return new();

        ApplyBox(_sectionEditModel.Entity.Box, shapeSection.Box);
        ApplyContainer(_sectionEditModel.Entity.Container, shapeSection.Container);

        var elements = new List<SectionElement>();
        var orderedElements = shapeSection.Elements.OrderBy(x => x.Ordinal).ToList();

        for (var index = 0; index < orderedElements.Count; index++)
        {
            var element = orderedElements[index];
            var response = await _sectionEditModel.CreateShapeElement(element.TypeId, index);

            if (!response.Ok || response.Value is null)
                return new() { Errors = response.Errors };

            var nextElement = response.Value;

            ApplyElement(nextElement, element, index);
            elements.Add(nextElement);
        }

        await _sectionEditModel.ReplaceElements(elements);

        return new();
    }

    private static void ApplyElement(SectionElement target, SectionElement source, Int32 ordinal)
    {
        target.Ordinal = ordinal;
        target.TypeId = source.TypeId;
        target.RequireInteraction = source.RequireInteraction;

        ApplyBox(target.Box, source.Box);
        ApplyItem(target.Item, source.Item);

        target.ConfigType = source.ConfigType;
        target.ConfigJson = source.ConfigJson;
    }

    internal static void ApplyBox(Box target, Box source)
    {
        target.BackgroundColor = source.BackgroundColor;
        target.BackgroundImageFile = source.BackgroundImageFile.DeepClone();
        target.BorderColor = source.BorderColor;
        target.BorderRadius = source.BorderRadius;
        target.BorderThickness = source.BorderThickness;
        target.BorderThicknessTop = source.BorderThicknessTop;
        target.BorderThicknessLeft = source.BorderThicknessLeft;
        target.BorderThicknessRight = source.BorderThicknessRight;
        target.BorderThicknessBottom = source.BorderThicknessBottom;
        target.CustomFontIndex = source.CustomFontIndex;
        target.FontSize = source.FontSize;
        target.FontWeight = source.FontWeight;
        target.ForegroundColor = source.ForegroundColor;
        target.MarginBottom = source.MarginBottom;
        target.MarginLeft = source.MarginLeft;
        target.MarginRight = source.MarginRight;
        target.MarginTop = source.MarginTop;
        target.PaddingBottom = source.PaddingBottom;
        target.PaddingLeft = source.PaddingLeft;
        target.PaddingRight = source.PaddingRight;
        target.PaddingTop = source.PaddingTop;
        target.ShadowBlurRadius = source.ShadowBlurRadius;
        target.ShadowColor = source.ShadowColor;
        target.ShadowOffsetX = source.ShadowOffsetX;
        target.ShadowOffsetY = source.ShadowOffsetY;
        target.ShadowSpreadRadius = source.ShadowSpreadRadius;
        target.TextShadowBlurRadius = source.TextShadowBlurRadius;
        target.TextShadowColor = source.TextShadowColor;
        target.TextShadowOffsetX = source.TextShadowOffsetX;
        target.TextShadowOffsetY = source.TextShadowOffsetY;
        target.HeadingLineHeight = source.HeadingLineHeight;
        target.ParagraphLineHeight = source.ParagraphLineHeight;
    }

    internal static void ApplyItem(Item target, Item source)
    {
        target.BasisId = source.BasisId;
        target.BasisAmount = source.BasisAmount;
        target.Grow = source.Grow;
        target.Shrink = source.Shrink;
        target.AlignSelfId = source.AlignSelfId;
        target.MaxWidth = source.MaxWidth;
        target.MinWidth = source.MinWidth;
        target.Width = source.Width;
    }

    internal static void ApplyContainer(DataContainer target, DataContainer source)
    {
        target.DirectionId = source.DirectionId;
        target.WrapId = source.WrapId;
        target.JustifyContentId = source.JustifyContentId;
        target.AlignItemsId = source.AlignItemsId;
        target.AlignContentId = source.AlignContentId;
        target.Gap = source.Gap;
    }
}

public class SectionShapeChoice : Described
{
    public Section Section
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    internal List<SectionShapeVariantTarget> VariantBoxes { get; } = [];

    public Boolean ReplacesContent => Section.Elements.HasItems();

    public Section BuildSection(Boolean bordered, Boolean rounded, Boolean shadowed)
    {
        var section = Section.DeepClone();

        foreach (var variantBox in VariantBoxes)
        {
            var box = variantBox.FindBox(section);

            if (box is null)
                continue;

            box.BorderRadius = rounded && variantBox.RoundedBorderRadius.HasSomething()
                ? variantBox.RoundedBorderRadius
                : null;

            if (bordered && variantBox.Border is not null)
                variantBox.Border.Apply(box);
            else
                SectionShapeVariantBorder.Clear(box);

            if (shadowed && variantBox.Shadow is not null)
                variantBox.Shadow.Apply(box);
            else
                SectionShapeVariantShadow.Clear(box);
        }

        return section;
    }
}

internal enum SectionShapeVariantScopes
{
    Section,
    Element,
    MultimediaItem,
}

internal class SectionShapeVariantTarget
{
    public SectionShapeVariantScopes Scope { get; set; }
    public Int32? ElementOrdinal { get; set; }
    public Int32? MultimediaItemOrdinal { get; set; }
    public String? RoundedBorderRadius { get; set; }
    public SectionShapeVariantBorder? Border { get; set; }
    public SectionShapeVariantShadow? Shadow { get; set; }

    public Box? FindBox(Section section)
    {
        return Scope switch
        {
            SectionShapeVariantScopes.Section => section.Box,
            SectionShapeVariantScopes.Element => section.Elements.FirstOrDefault(x => x.Ordinal == ElementOrdinal)?.Box,
            SectionShapeVariantScopes.MultimediaItem => section.Elements
                .FirstOrDefault(x => x.Ordinal == ElementOrdinal)?
                .As<MultimediaElement>()?
                .MultimediaItems
                .FirstOrDefault(x => x.Ordinal == MultimediaItemOrdinal)?
                .Box,
            _ => null,
        };
    }
}

internal class SectionShapeVariantBorder
{
    public String? Thickness { get; set; }
    public String? ThicknessTop { get; set; }
    public String? ThicknessLeft { get; set; }
    public String? ThicknessRight { get; set; }
    public String? ThicknessBottom { get; set; }

    public void Apply(Box box)
    {
        box.BorderColor = null;
        box.BorderThickness = Thickness;
        box.BorderThicknessTop = ThicknessTop;
        box.BorderThicknessLeft = ThicknessLeft;
        box.BorderThicknessRight = ThicknessRight;
        box.BorderThicknessBottom = ThicknessBottom;
    }

    public static void Clear(Box box)
    {
        box.BorderColor = null;
        box.BorderThickness = null;
        box.BorderThicknessTop = null;
        box.BorderThicknessLeft = null;
        box.BorderThicknessRight = null;
        box.BorderThicknessBottom = null;
    }
}

internal class SectionShapeVariantShadow
{
    public String? BlurRadius { get; set; }
    public String? Color { get; set; }
    public String? OffsetX { get; set; }
    public String? OffsetY { get; set; }
    public String? SpreadRadius { get; set; }

    public void Apply(Box box)
    {
        box.ShadowBlurRadius = BlurRadius;
        box.ShadowColor = Color;
        box.ShadowOffsetX = OffsetX;
        box.ShadowOffsetY = OffsetY;
        box.ShadowSpreadRadius = SpreadRadius;
    }

    public static void Clear(Box box)
    {
        box.ShadowBlurRadius = null;
        box.ShadowColor = null;
        box.ShadowOffsetX = null;
        box.ShadowOffsetY = null;
        box.ShadowSpreadRadius = null;
    }
}