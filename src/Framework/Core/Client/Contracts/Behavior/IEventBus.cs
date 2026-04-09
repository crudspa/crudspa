namespace Crudspa.Framework.Core.Client.Contracts.Behavior;

public interface IEventBus
{
    void Subscribe(Object subscriber);
    void Unsubscribe(Object subscriber);
    Task Publish(Object message);
}