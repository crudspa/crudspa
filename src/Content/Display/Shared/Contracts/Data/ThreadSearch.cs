namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class ThreadSearch : Search
{
    private void HandlePostedRangeChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(PostedRange));
    private DateRange _postedRange;

    public ThreadSearch()
    {
        _postedRange = new();
        _postedRange.PropertyChanged += HandlePostedRangeChanged;
    }

    public override void Dispose()
    {
        _postedRange.PropertyChanged -= HandlePostedRangeChanged;
        base.Dispose();
    }

    public DateRange PostedRange
    {
        get => _postedRange;
        set => SetProperty(ref _postedRange, value);
    }
}