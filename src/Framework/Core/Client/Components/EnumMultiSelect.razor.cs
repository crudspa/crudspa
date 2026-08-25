using G = System.Collections.Generic;

namespace Crudspa.Framework.Core.Client.Components;

public partial class EnumMultiSelect<TEnum> where TEnum : struct, Enum
{
    [Parameter] public G.List<TEnum> SelectedValues { get; set; } = null!;
    [Parameter] public EventCallback<G.List<TEnum>> SelectedValuesChanged { get; set; }

    private static IEnumerable<EnumOption<TEnum>> Options => Enum
        .GetValues<TEnum>()
        .Select(value => new EnumOption<TEnum>(value, value.GetLabel()));
}

public sealed record EnumOption<TEnum>(TEnum Value, String Name)
    where TEnum : struct, Enum;