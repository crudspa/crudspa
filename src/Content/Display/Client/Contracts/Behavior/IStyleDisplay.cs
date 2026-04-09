namespace Crudspa.Content.Display.Client.Contracts.Behavior;

public interface IStyleDisplay : IConfigDisplay
{
    Guid? ContentPortalId { get; set; }
}