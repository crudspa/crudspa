using System.Reflection;

namespace Crudspa.Framework.Core.Client.Services;

public class EventBusCore : IEventBus
{
    private readonly List<Handler> _handlers = [];

    public void Subscribe(Object subscriber) => Subscribe(subscriber, x => x());

    private void Subscribe(Object subscriber, Func<Func<Task>, Task> invoker)
    {
        lock (_handlers)
        {
            if (_handlers.Any(x => x.Matches(subscriber)))
                return;

            _handlers.Add(new(subscriber, invoker));
        }
    }

    public void Unsubscribe(Object subscriber)
    {
        lock (_handlers)
        {
            var found = _handlers.FirstOrDefault(x => x.Matches(subscriber));

            if (found is not null)
                _handlers.Remove(found);
        }
    }

    public Task Publish(Object message) => Publish(message, f => f());

    private Task Publish(Object message, Func<Func<Task>, Task> invoker)
    {
        Handler[] handlers;

        lock (_handlers)
            handlers = _handlers.ToArray();

        return invoker(async () =>
        {
            var messageType = message.GetType();

            var tasks = handlers.Select(x => x.Handle(messageType, message));

            await Task.WhenAll(tasks);

            var dead = handlers.Where(x => x.IsDead).ToList();

            if (dead.Count > 0)
                lock (_handlers)
                    dead.Apply(x => _handlers.Remove(x));
        });
    }

    private class Handler
    {
        private readonly WeakReference _reference;
        private readonly Func<Func<Task>, Task> _invoker;
        private readonly Dictionary<Type, MethodInfo> _supported = new();

        public Boolean IsDead => _reference.Target is null;
        public Boolean Matches(Object instance) => _reference.Target == instance;

        public Handler(Object handler, Func<Func<Task>, Task> invoker)
        {
            _reference = new(handler);
            _invoker = invoker;

            var interfaces = handler.GetType().GetTypeInfo().ImplementedInterfaces
                .Where(x => x.GetTypeInfo().IsGenericType && x.GetGenericTypeDefinition() == typeof(IHandle<>));

            foreach (var face in interfaces)
            {
                var type = face.GetTypeInfo().GenericTypeArguments[0];
                var method = face.GetRuntimeMethod("Handle", [type]);

                if (method is not null)
                    _supported[type] = method;
            }
        }

        public Task Handle(Type messageType, Object message)
        {
            var target = _reference.Target;

            if (target is null)
                return Task.FromResult(false);

            return _invoker(() =>
            {
                var tasks = _supported
                    .Where(x => x.Key.GetTypeInfo().IsAssignableFrom(messageType.GetTypeInfo()))
                    .Select(x => x.Value.Invoke(target, [message]))
                    .Select(x => (Task)x!)
                    .ToList();

                return Task.WhenAll(tasks);
            });
        }
    }
}