namespace Crudspa.Framework.Core.Client.Models;

public class PaneMoveModel(IScrollService scrollService, ISegmentService segmentService, IPaneService paneService)
    : ModalModel(scrollService)
{
    private Guid? _paneId;
    private Guid? _currentSegmentId;

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

    public async Task ShowModal(Guid? paneId, Guid? currentSegmentId)
    {
        _paneId = paneId;
        _currentSegmentId = currentSegmentId;

        Portals = [];
        MoveToId = currentSegmentId;

        await Show();

        var response = await WithWaiting("Fetching...", () => segmentService.FetchTree(new()));

        if (!response.Ok)
            return;

        Portals = response.Value.ToObservable();

        if (currentSegmentId.HasValue)
            Portals.Select(Portals.FindParent(currentSegmentId));

        RaisePropertyChanged(nameof(Portals));
        RaisePropertyChanged(nameof(MoveToId));
    }

    public async Task MovePane()
    {
        if (_paneId is null)
            return;

        Alerts.Clear();

        var destinationSegmentId = ResolveDestinationSegmentId();

        if (!destinationSegmentId.HasValue)
        {
            Alerts.Add(new()
            {
                Type = Alert.AlertType.Error,
                Errors =
                [
                    new() { Message = "Select a destination segment." },
                ],
            });

            return;
        }

        if (destinationSegmentId.Equals(_currentSegmentId))
        {
            await Hide();
            return;
        }

        var pane = new Pane
        {
            Id = _paneId,
            SegmentId = destinationSegmentId,
        };

        var response = await WithWaiting("Moving...", () => paneService.Move(new(pane)));

        if (response.Ok)
            await Hide();
    }

    private Guid? ResolveDestinationSegmentId()
    {
        if (MoveToId is null)
            return null;

        foreach (var portal in Portals)
        {
            if (portal.Id.Equals(MoveToId))
                return null;

            if (portal.HasChild(MoveToId))
                return MoveToId;
        }

        return null;
    }
}