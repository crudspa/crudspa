namespace Crudspa.Content.Design.Shared.Contracts.Data;

public class EmailSearch : Search
{
    private void HandleSendRangeChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(SendRange));
    private void HandleProcessedRangeChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(ProcessedRange));
    private DateRange _sendRange;
    private DateRange _processedRange;

    public EmailSearch()
    {
        _sendRange = new();
        _sendRange.PropertyChanged += HandleSendRangeChanged;
        _processedRange = new();
        _processedRange.PropertyChanged += HandleProcessedRangeChanged;
    }

    public override void Dispose()
    {
        _sendRange.PropertyChanged -= HandleSendRangeChanged;
        _processedRange.PropertyChanged -= HandleProcessedRangeChanged;
        base.Dispose();
    }

    public DateRange SendRange
    {
        get => _sendRange;
        set => SetProperty(ref _sendRange, value);
    }

    public DateRange ProcessedRange
    {
        get => _processedRange;
        set => SetProperty(ref _processedRange, value);
    }
}