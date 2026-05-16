namespace Crudspa.Content.Design.Shared.Contracts.Data;

public class SmsPreferenceSearch : Search
{
    private void HandleStatusChangedRangeChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(StatusChangedRange));

    private DateRange _statusChangedRange;

    public SmsPreferenceSearch()
    {
        _statusChangedRange = new();
        _statusChangedRange.PropertyChanged += HandleStatusChangedRangeChanged;
    }

    public override void Dispose()
    {
        _statusChangedRange.PropertyChanged -= HandleStatusChangedRangeChanged;
        base.Dispose();
    }

    public DateRange StatusChangedRange
    {
        get => _statusChangedRange;
        set => SetProperty(ref _statusChangedRange, value);
    }
}