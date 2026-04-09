namespace Crudspa.Framework.Core.Shared.Contracts.Behavior;

public interface IValidates
{
    List<Error> Validate();
}