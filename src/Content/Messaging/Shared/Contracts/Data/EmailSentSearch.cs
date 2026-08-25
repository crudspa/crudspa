namespace Crudspa.Content.Messaging.Shared.Contracts.Data;

public class EmailSentSearch : Search
{
    private void HandleProcessedRangeChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(ProcessedRange));

    private DateRange _processedRange;

    public EmailSentSearch()
    {
        _processedRange = new();
        _processedRange.PropertyChanged += HandleProcessedRangeChanged;
    }

    public override void Dispose()
    {
        _processedRange.PropertyChanged -= HandleProcessedRangeChanged;
        base.Dispose();
    }

    public DateRange ProcessedRange
    {
        get => _processedRange;
        set => SetProperty(ref _processedRange, value);
    }

    public List<EmailSent.Statuses> Statuses
    {
        get;
        set => SetProperty(ref field, value);
    } = [];
}