namespace Crudspa.Education.Common.Client.Models;

public class LineModel
{
    public Boolean? IsLabel { get; set; }
    public String? LabelText { get; set; }
    public Boolean? IsInput { get; set; }
    public Boolean? IsTallInput { get; set; }
    public String? EntryText { get; set; } = String.Empty;
    public Boolean? IsChoice { get; set; }
    public DestinationModel? DestinationModel { get; set; }
    public List<Int32>? ValidIndexes { get; set; }
    public Boolean? IsValidating { get; set; }

    public Boolean NeedsAttention()
    {
        return IsValidating.HasValue && IsValidating.Value
            && ((IsInput.HasValue && IsInput.Value) || IsTallInput.HasValue && IsTallInput.Value && String.IsNullOrEmpty(EntryText)
                || (IsChoice.HasValue && IsChoice.Value && DestinationModel?.PlacedPiece is null));
    }
}