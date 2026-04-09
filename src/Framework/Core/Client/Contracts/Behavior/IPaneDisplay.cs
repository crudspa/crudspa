namespace Crudspa.Framework.Core.Client.Contracts.Behavior;

public interface IPaneDisplay : IConfigDisplay
{
    String? Path { get; set; }
    Guid? Id { get; set; }
    Boolean IsNew { get; set; }
}