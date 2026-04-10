namespace Crudspa.Content.Design.Client.Models;

public static class SettingsTransferModel
{
    private static readonly HashSet<String> BoxLabels = new(StringComparer.OrdinalIgnoreCase)
    {
        "Background Color",
        "Background Image",
        "Border Color",
        "Border Radius",
        "Border Thickness",
        "Border Thickness Top",
        "Border Thickness Left",
        "Border Thickness Right",
        "Border Thickness Bottom",
        "Font",
        "Font Size",
        "Font Weight",
        "Foreground Color",
        "Heading Line Height",
        "Margin Top",
        "Margin Left",
        "Margin Right",
        "Margin Bottom",
        "Padding Top",
        "Padding Left",
        "Padding Right",
        "Padding Bottom",
        "Paragraph Line Height",
        "Shadow Blur Radius",
        "Shadow Color",
        "Shadow Offset X",
        "Shadow Offset Y",
        "Shadow Spread Radius",
        "Text Shadow Blur Radius",
        "Text Shadow Color",
        "Text Shadow Offset X",
        "Text Shadow Offset Y",
    };

    private static readonly HashSet<String> ItemLabels = new(StringComparer.OrdinalIgnoreCase)
    {
        "Align Self",
        "Amount",
        "Basis",
        "Grow",
        "Max Width",
        "Min Width",
        "Shrink",
        "Width",
    };

    private static readonly HashSet<String> ContainerLabels = new(StringComparer.OrdinalIgnoreCase)
    {
        "Align Content",
        "Align Items",
        "Direction",
        "Gap",
        "Justify Content",
        "Wrap",
    };

    public static Boolean TryApplyDescription(String? description,
        BoxModel? boxModel = null,
        ItemModel? itemModel = null,
        ContainerModel? containerModel = null)
    {
        if (description.HasNothing())
            return false;

        if (description!.IsBasically("Default") || description.IsBasically("None"))
        {
            if (boxModel is null)
                return false;

            ApplyBox(boxModel, new Box());
            return true;
        }

        var values = ParseValues(description);

        var applied = false;

        if (boxModel is not null && values.Keys.Any(BoxLabels.Contains))
        {
            ApplyBox(boxModel, values);
            applied = true;
        }

        if (itemModel is not null && values.Keys.Any(ItemLabels.Contains))
        {
            ApplyItem(itemModel, values);
            applied = true;
        }

        if (containerModel is not null && values.Keys.Any(ContainerLabels.Contains))
        {
            ApplyContainer(containerModel, values);
            applied = true;
        }

        return applied;
    }

    private static Dictionary<String, String> ParseValues(String description)
    {
        Dictionary<String, String> values = new(StringComparer.OrdinalIgnoreCase);

        foreach (var part in description.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (!part.HasSomething())
                continue;

            var separatorIndex = part.IndexOf(':');
            if (separatorIndex <= 0)
                continue;

            var key = part[..separatorIndex].Trim();
            var value = part[(separatorIndex + 1)..].Trim();

            if (key.HasSomething())
                values[key] = value;
        }

        return values;
    }

    private static void ApplyBox(BoxModel model, Box source)
    {
        model.Box.BorderColor = source.BorderColor;
        model.Box.BorderRadius = source.BorderRadius;
        model.Box.BorderThickness = source.BorderThickness;
        model.Box.BorderThicknessTop = source.BorderThicknessTop;
        model.Box.BorderThicknessLeft = source.BorderThicknessLeft;
        model.Box.BorderThicknessRight = source.BorderThicknessRight;
        model.Box.BorderThicknessBottom = source.BorderThicknessBottom;
        model.Box.CustomFontIndex = source.CustomFontIndex;
        model.Box.FontSize = source.FontSize;
        model.Box.FontWeight = source.FontWeight;
        model.Box.BackgroundColor = source.BackgroundColor;
        model.Box.BackgroundImageFile = source.BackgroundImageFile;
        model.Box.ForegroundColor = source.ForegroundColor;
        model.Box.MarginTop = source.MarginTop;
        model.Box.MarginLeft = source.MarginLeft;
        model.Box.MarginRight = source.MarginRight;
        model.Box.MarginBottom = source.MarginBottom;
        model.Box.PaddingTop = source.PaddingTop;
        model.Box.PaddingLeft = source.PaddingLeft;
        model.Box.PaddingRight = source.PaddingRight;
        model.Box.PaddingBottom = source.PaddingBottom;
        model.Box.ShadowBlurRadius = source.ShadowBlurRadius;
        model.Box.ShadowColor = source.ShadowColor;
        model.Box.ShadowOffsetX = source.ShadowOffsetX;
        model.Box.ShadowOffsetY = source.ShadowOffsetY;
        model.Box.ShadowSpreadRadius = source.ShadowSpreadRadius;
        model.Box.HeadingLineHeight = source.HeadingLineHeight;
        model.Box.ParagraphLineHeight = source.ParagraphLineHeight;
        model.Box.TextShadowBlurRadius = source.TextShadowBlurRadius;
        model.Box.TextShadowColor = source.TextShadowColor;
        model.Box.TextShadowOffsetX = source.TextShadowOffsetX;
        model.Box.TextShadowOffsetY = source.TextShadowOffsetY;
        model.RefreshOverrideStates();
    }

    private static void ApplyBox(BoxModel model, IReadOnlyDictionary<String, String> values)
    {
        model.Box.BorderColor = GetNullable(values, "Border Color");
        model.Box.BorderRadius = GetNullable(values, "Border Radius");
        model.Box.BorderThickness = GetNullable(values, "Border Thickness");
        model.Box.BorderThicknessTop = GetNullable(values, "Border Thickness Top");
        model.Box.BorderThicknessLeft = GetNullable(values, "Border Thickness Left");
        model.Box.BorderThicknessRight = GetNullable(values, "Border Thickness Right");
        model.Box.BorderThicknessBottom = GetNullable(values, "Border Thickness Bottom");
        model.Box.CustomFontIndex = ResolveFont(values, model.Box.CustomFontIndex);
        model.Box.FontSize = GetNullable(values, "Font Size");
        model.Box.FontWeight = GetNullable(values, "Font Weight");
        model.Box.BackgroundColor = GetNullable(values, "Background Color");
        model.Box.ForegroundColor = GetNullable(values, "Foreground Color");
        model.Box.MarginTop = GetNullable(values, "Margin Top");
        model.Box.MarginLeft = GetNullable(values, "Margin Left");
        model.Box.MarginRight = GetNullable(values, "Margin Right");
        model.Box.MarginBottom = GetNullable(values, "Margin Bottom");
        model.Box.PaddingTop = GetNullable(values, "Padding Top");
        model.Box.PaddingLeft = GetNullable(values, "Padding Left");
        model.Box.PaddingRight = GetNullable(values, "Padding Right");
        model.Box.PaddingBottom = GetNullable(values, "Padding Bottom");
        model.Box.ShadowBlurRadius = GetNullable(values, "Shadow Blur Radius");
        model.Box.ShadowColor = GetNullable(values, "Shadow Color");
        model.Box.ShadowOffsetX = GetNullable(values, "Shadow Offset X");
        model.Box.ShadowOffsetY = GetNullable(values, "Shadow Offset Y");
        model.Box.ShadowSpreadRadius = GetNullable(values, "Shadow Spread Radius");
        model.Box.HeadingLineHeight = GetNullable(values, "Heading Line Height");
        model.Box.ParagraphLineHeight = GetNullable(values, "Paragraph Line Height");
        model.Box.TextShadowBlurRadius = GetNullable(values, "Text Shadow Blur Radius");
        model.Box.TextShadowColor = GetNullable(values, "Text Shadow Color");
        model.Box.TextShadowOffsetX = GetNullable(values, "Text Shadow Offset X");
        model.Box.TextShadowOffsetY = GetNullable(values, "Text Shadow Offset Y");

        if (!values.ContainsKey("Background Image"))
            model.Box.BackgroundImageFile = new();

        model.RefreshOverrideStates();
    }

    private static Box.CustomFonts ResolveFont(IReadOnlyDictionary<String, String> values, Box.CustomFonts current)
    {
        if (!TryGetValue(values, "Font", out var fontValue))
            return Box.CustomFonts.Default;

        foreach (var candidate in Enum.GetValues<Box.CustomFonts>())
        {
            if (candidate.GetLabel().IsBasically(fontValue))
                return candidate;
        }

        return current;
    }

    private static void ApplyItem(ItemModel model, IReadOnlyDictionary<String, String> values)
    {
        model.Item.BasisId = ResolveNamedId(values, "Basis", model.BasisNames, model.Item.BasisId, BasisIds.Auto);
        model.Item.BasisAmount = GetNullable(values, "Amount");
        model.Item.Grow = GetNullable(values, "Grow") ?? model.Item.Grow;
        model.Item.Shrink = GetNullable(values, "Shrink") ?? model.Item.Shrink;
        model.Item.AlignSelfId = ResolveNamedId(values, "Align Self", model.AlignSelfNames, model.Item.AlignSelfId, AlignSelfIds.Auto);
        model.Item.MaxWidth = GetNullable(values, "Max Width");
        model.Item.MinWidth = GetNullable(values, "Min Width");
        model.Item.Width = GetNullable(values, "Width");
        model.RefreshOverrideStates();
    }

    private static void ApplyContainer(ContainerModel model, IReadOnlyDictionary<String, String> values)
    {
        model.Container.DirectionId = ResolveNamedId(values, "Direction", model.DirectionNames, model.Container.DirectionId, model.Container.DirectionId);
        model.Container.WrapId = ResolveNamedId(values, "Wrap", model.WrapNames, model.Container.WrapId, model.Container.WrapId);
        model.Container.JustifyContentId = ResolveNamedId(values, "Justify Content", model.JustifyContentNames, model.Container.JustifyContentId, model.Container.JustifyContentId);
        model.Container.AlignItemsId = ResolveNamedId(values, "Align Items", model.AlignItemsNames, model.Container.AlignItemsId, model.Container.AlignItemsId);
        model.Container.AlignContentId = ResolveNamedId(values, "Align Content", model.AlignContentNames, model.Container.AlignContentId, model.Container.AlignContentId);
        model.Container.Gap = GetNullable(values, "Gap");
    }

    private static Guid? ResolveNamedId(IReadOnlyDictionary<String, String> values,
        String key,
        IList<Orderable> names,
        Guid? current,
        Guid? defaultValue)
    {
        if (!TryGetValue(values, key, out var name))
            return defaultValue;

        if (!name.HasSomething())
            return defaultValue;

        return names.FindIdByName(name!) ?? current;
    }

    private static String? GetNullable(IReadOnlyDictionary<String, String> values, String key)
    {
        return TryGetValue(values, key, out var value)
            ? value
            : null;
    }

    private static Boolean TryGetValue(IReadOnlyDictionary<String, String> values, String key, out String? value)
    {
        if (values.TryGetValue(key, out var rawValue))
        {
            value = rawValue.HasSomething() ? rawValue : null;
            return true;
        }

        value = null;
        return false;
    }
}