namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class Search : Observable, IDisposable
{
    private void HandlePagedChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(Paged));
    private void HandleSortChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(Sort));

    private Paged _paged;
    private Sort _sort;

    public Search()
    {
        _paged = new();
        _paged.PropertyChanged += HandlePagedChanged;

        _sort = new();
        _sort.PropertyChanged += HandleSortChanged;
    }

    public virtual void Dispose()
    {
        _paged.PropertyChanged -= HandlePagedChanged;
        _sort.PropertyChanged -= HandleSortChanged;
    }

    public String? Text
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Paged Paged
    {
        get => _paged;
        set => SetProperty(ref _paged, value);
    }

    public Sort Sort
    {
        get => _sort;
        set => SetProperty(ref _sort, value);
    }

    public String? TimeZoneId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ParentId
    {
        get;
        set => SetProperty(ref field, value);
    }
}