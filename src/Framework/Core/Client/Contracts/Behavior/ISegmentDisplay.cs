namespace Crudspa.Framework.Core.Client.Contracts.Behavior;

public interface ISegmentDisplay : IConfigDisplay
{
    String? Path { get; set; }
    Guid? Id { get; set; }
    IEnumerable<NavPane>? Panes { get; set; }
}