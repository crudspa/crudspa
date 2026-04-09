namespace Crudspa.Content.Display.Shared.Contracts.Events;

public class ElementProgressPayload
{
    public ElementProgress Progress { get; set; } = null!;
}

public class ElementProgressUpdated : ElementProgressPayload;