namespace Crudspa.Framework.Jobs.Shared.Contracts.Data;

public class JobSearch : Search
{
    private void HandleAddedRangeChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(AddedRange));
    private DateRange _addedRange;

    public JobSearch()
    {
        _addedRange = new();
        _addedRange.PropertyChanged += HandleAddedRangeChanged;
    }

    public override void Dispose()
    {
        _addedRange.PropertyChanged -= HandleAddedRangeChanged;
        base.Dispose();
    }

    public List<Guid?> Types
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Guid?> Status
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Guid?> Devices
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Guid?> Schedules
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public DateRange AddedRange
    {
        get => _addedRange;
        set => SetProperty(ref _addedRange, value);
    }
}