namespace Crudspa.Framework.Core.Client.Contracts.Behavior;

public interface IFilterDesign : IConfigDesign
{
    IEnumerable<Named>? Values { get; set; }
}