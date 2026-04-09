using System.Collections.Concurrent;

namespace Crudspa.Content.Design.Server.Services;

public class ElementRepositoryFactoryContent(IServiceProvider serviceProvider)
    : IElementRepositoryFactory
{
    private readonly ConcurrentDictionary<String, IElementRepository> _elementRepositories = new();

    public IElementRepository Create(String className)
    {
        return _elementRepositories.GetOrAdd(className, key =>
        {
            var type = Type.GetType(key);

            if (type is null)
                throw new($"Type {key} could not be found.");

            var newRepository = (IElementRepository)ActivatorUtilities.CreateInstance(serviceProvider, type);

            return newRepository;
        });
    }
}