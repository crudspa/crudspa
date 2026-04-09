namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class RowData
{
    public Int32 Index { get; set; }
    public Dictionary<Int32, String> ColumnValues { get; set; } = null!;
}