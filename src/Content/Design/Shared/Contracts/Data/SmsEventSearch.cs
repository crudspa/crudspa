namespace Crudspa.Content.Design.Shared.Contracts.Data;

public class SmsEventSearch : Search
{
    private void HandleReceivedRangeChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(ReceivedRange));

    private DateRange _receivedRange;

    public SmsEventSearch()
    {
        _receivedRange = new();
        _receivedRange.PropertyChanged += HandleReceivedRangeChanged;
    }

    public override void Dispose()
    {
        _receivedRange.PropertyChanged -= HandleReceivedRangeChanged;
        base.Dispose();
    }

    public DateRange ReceivedRange
    {
        get => _receivedRange;
        set => SetProperty(ref _receivedRange, value);
    }
}