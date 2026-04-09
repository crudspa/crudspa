namespace Crudspa.Framework.Core.Client.Contracts.Behavior;

public interface IPaneDesign : IConfigDesign
{
    String? Path { get; set; }
    EventCallback ConfigUpdated { get; set; }
    Task<Boolean> PrepareForSave();
}