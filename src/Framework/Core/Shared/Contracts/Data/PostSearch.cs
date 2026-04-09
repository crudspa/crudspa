namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class PostSearch : Search
{
    private void HandlePublishedRangeChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(PublishedRange));
    private void HandleRevisedRangeChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(RevisedRange));
    private DateRange _publishedRange;
    private DateRange _revisedRange;

    public PostSearch()
    {
        _publishedRange = new();
        _publishedRange.PropertyChanged += HandlePublishedRangeChanged;
        _revisedRange = new();
        _revisedRange.PropertyChanged += HandleRevisedRangeChanged;
    }

    public override void Dispose()
    {
        _publishedRange.PropertyChanged -= HandlePublishedRangeChanged;
        _revisedRange.PropertyChanged -= HandleRevisedRangeChanged;
        base.Dispose();
    }

    public List<Guid?> Status
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public DateRange PublishedRange
    {
        get => _publishedRange;
        set => SetProperty(ref _publishedRange, value);
    }

    public DateRange RevisedRange
    {
        get => _revisedRange;
        set => SetProperty(ref _revisedRange, value);
    }
}