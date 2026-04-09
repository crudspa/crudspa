namespace Crudspa.Education.Publisher.Shared.Contracts.Data;

public class AssessmentSearch : Search
{
    private void HandleAvailableStartRangeChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(AvailableStartRange));

    private void HandleAvailableEndRangeChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(AvailableEndRange));

    private DateRange _availableStartRange;

    private DateRange _availableEndRange;

    public AssessmentSearch()
    {
        _availableStartRange = new();
        _availableStartRange.PropertyChanged += HandleAvailableStartRangeChanged;
        _availableEndRange = new();
        _availableEndRange.PropertyChanged += HandleAvailableEndRangeChanged;
    }

    public override void Dispose()
    {
        _availableStartRange.PropertyChanged -= HandleAvailableStartRangeChanged;
        _availableEndRange.PropertyChanged -= HandleAvailableEndRangeChanged;
        base.Dispose();
    }

    public List<Guid?> Grades
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Guid?> Status
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public DateRange AvailableStartRange
    {
        get => _availableStartRange;
        set => SetProperty(ref _availableStartRange, value);
    }

    public DateRange AvailableEndRange
    {
        get => _availableEndRange;
        set => SetProperty(ref _availableEndRange, value);
    }

    public List<Guid?> Categories
    {
        get;
        set => SetProperty(ref field, value);
    } = [];
}