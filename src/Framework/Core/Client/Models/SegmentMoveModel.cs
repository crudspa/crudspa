namespace Crudspa.Framework.Core.Client.Models;

public class SegmentMoveModel(IScrollService scrollService, ISegmentService segmentService)
    : ModalModel(scrollService)
{
    private Guid? _segmentId;

    public ObservableCollection<Expandable> Portals
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public Guid? MoveToId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public async Task ShowModal(Guid? segmentId)
    {
        _segmentId = segmentId;

        Portals = [];

        await Show();

        var response = await WithWaiting("Fetching...", () => segmentService.FetchTree(new()));

        if (response.Ok)
        {
            Portals = response.Value.ToObservable();
            Portals.Select(Portals.FindParent(segmentId));
        }

        RaisePropertyChanged(nameof(Portals));
    }

    public async Task MoveSegment()
    {
        var segment = SetIds();

        var response = await WithWaiting("Moving...", () => segmentService.Move(new(segment)));

        if (response.Ok)
            await Hide();
    }

    private Segment SetIds()
    {
        var segment = new Segment { Id = _segmentId };

        foreach (var portal in Portals)
        {
            if (portal.Id.Equals(MoveToId))
            {
                segment.PortalId = MoveToId;
                segment.ParentId = null;
                return segment;
            }

            if (portal.HasChild(MoveToId))
            {
                segment.PortalId = portal.Id;
                segment.ParentId = MoveToId;
                return segment;
            }
        }

        throw new("ID not found in tree.");
    }
}