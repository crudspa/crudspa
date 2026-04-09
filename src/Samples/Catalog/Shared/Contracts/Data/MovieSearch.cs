namespace Crudspa.Samples.Catalog.Shared.Contracts.Data;

public class MovieSearch : Search
{
    private void HandleReleasedRangeChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(ReleasedRange));

    private DateRange _releasedRange;

    public MovieSearch()
    {
        _releasedRange = new();
        _releasedRange.PropertyChanged += HandleReleasedRangeChanged;
    }

    public override void Dispose()
    {
        _releasedRange.PropertyChanged -= HandleReleasedRangeChanged;
        base.Dispose();
    }

    public List<Guid?> Genres
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Guid?> Ratings
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public DateRange ReleasedRange
    {
        get => _releasedRange;
        set => SetProperty(ref _releasedRange, value);
    }
}