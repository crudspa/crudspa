namespace Crudspa.Framework.Core.Shared.Contracts.Behavior;

public interface IHandle<in TPayload>
{
    Task Handle(TPayload payload);
}