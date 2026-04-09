namespace Crudspa.Framework.Core.Client.Contracts.Data;

public class Tab
{
    public required String? Key { get; init; }
    public required String? Title { get; init; }
    public Boolean Lazy { get; init; }
    public Boolean Loaded { get; set; }
    public RenderFragment? Content { get; init; }
    public String? PaneTypeDisplayView { get; init; }
    public Guid? Id { get; set; }
    public Boolean IsNew { get; init; }
    public String? ConfigJson { get; init; }
}

public sealed record TabScope(Int32 Depth)
{
    public String Key => Depth == 1 ? "tab" : $"tab{Depth:D}";
    public static TabScope Root => new(1);
    public TabScope Next() => new(Depth + 1);
}