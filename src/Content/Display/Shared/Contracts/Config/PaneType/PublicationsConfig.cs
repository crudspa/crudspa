namespace Crudspa.Content.Display.Shared.Contracts.Config.PaneType;

public class PublicationsConfig : Observable
{
    public enum SortOrders { Manual, Newest }

    public Boolean Grouped { get; set; } = true;
    public Guid? GroupId { get; set; }
    public SortOrders SortOrder { get; set; }
}