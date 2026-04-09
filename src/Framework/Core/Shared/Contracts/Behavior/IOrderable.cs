namespace Crudspa.Framework.Core.Shared.Contracts.Behavior;

public interface IOrderable : IUnique
{
    Int32? Ordinal { get; set; }
}