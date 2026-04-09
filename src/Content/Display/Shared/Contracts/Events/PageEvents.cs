namespace Crudspa.Content.Display.Shared.Contracts.Events;

public class PagePayload
{
    public Guid? Id { get; set; }
    public Guid? BinderId { get; set; }
}

public class PageAdded : PagePayload;

public class PageSaved : PagePayload;

public class PageRemoved : PagePayload;

public class PagesReordered : PagePayload;

public class PageContentChanged : PagePayload;