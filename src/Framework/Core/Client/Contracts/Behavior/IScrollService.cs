namespace Crudspa.Framework.Core.Client.Contracts.Behavior;

public interface IScrollService
{
    Task ToTop();
    Task ToBottom();
    Task ToId(String id);
    Task ToId(Guid id);
}