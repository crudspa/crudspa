namespace Crudspa.Content.Design.Server.Contracts.Behavior;

public interface IElementRepositoryFactory
{
    IElementRepository Create(String className);
}