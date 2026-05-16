namespace Crudspa.Content.Design.Shared.Contracts.Data;

public class SmsMessageSearch : Search
{
    private void HandleOccurredRangeChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(OccurredRange));

    private DateRange _occurredRange;

    public SmsMessageSearch()
    {
        _occurredRange = new();
        _occurredRange.PropertyChanged += HandleOccurredRangeChanged;
    }

    public override void Dispose()
    {
        _occurredRange.PropertyChanged -= HandleOccurredRangeChanged;
        base.Dispose();
    }

    public DateRange OccurredRange
    {
        get => _occurredRange;
        set => SetProperty(ref _occurredRange, value);
    }

    public SmsMessage.Directions? Direction
    {
        get;
        set => SetProperty(ref field, value);
    }
}