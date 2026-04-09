namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class Session
{
    public Guid? Id { get; set; }
    public User? User { get; set; }
    public Guid? PortalId { get; set; }
    public ObservableCollection<Guid> Permissions { get; set; } = [];
    public ObservableCollection<NavSegment> Segments { get; set; } = [];
    public ObservableCollection<Screen> Screens { get; set; } = [];
}