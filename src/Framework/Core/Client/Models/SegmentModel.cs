namespace Crudspa.Framework.Core.Client.Models;

public class SegmentModel : Observable, IDisposable, INamed, IOrderable
{
    private void HandleSegmentChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(Segment));

    private Segment _segment;

    public String? Name => Segment.Name;

    public SegmentModel(Segment segment)
    {
        _segment = segment;
        _segment.PropertyChanged += HandleSegmentChanged;
    }

    public void Dispose()
    {
        _segment.PropertyChanged -= HandleSegmentChanged;
    }

    public Guid? Id
    {
        get => _segment.Id;
        set => _segment.Id = value;
    }

    public Int32? Ordinal
    {
        get => _segment.Ordinal;
        set => _segment.Ordinal = value;
    }

    public Segment Segment
    {
        get => _segment;
        set => SetProperty(ref _segment, value);
    }

    public String EffectivePermissionName => Segment.PermissionName ?? "[Public]";
}