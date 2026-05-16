namespace Crudspa.Content.Display.Client.Contracts.Behavior;

public interface IBinderDisplay : IConfigDisplay
{
    String? Path { get; set; }
    Guid? Id { get; set; }
    EventCallback BinderCompleted { get; set; }
    RenderFragment<Page>? PageHeader { get; set; }
    Boolean Shadowed { get; set; }
}