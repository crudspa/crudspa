namespace Crudspa.Framework.Core.Shared.Contracts.Behavior;

public interface INamed : IUnique
{
    String? Name { get; }
}