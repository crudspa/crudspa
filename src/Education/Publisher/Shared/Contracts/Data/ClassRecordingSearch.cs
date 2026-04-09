namespace Crudspa.Education.Publisher.Shared.Contracts.Data;

public class ClassRecordingSearch : Search
{
    private void HandleUploadedRangeChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(UploadedRange));

    private DateRange _uploadedRange;

    public ClassRecordingSearch()
    {
        _uploadedRange = new();
        _uploadedRange.PropertyChanged += HandleUploadedRangeChanged;
    }

    public override void Dispose()
    {
        _uploadedRange.PropertyChanged -= HandleUploadedRangeChanged;
        base.Dispose();
    }

    public DateRange UploadedRange
    {
        get => _uploadedRange;
        set => SetProperty(ref _uploadedRange, value);
    }
}