namespace Crudspa.Framework.Core.Client.Contracts.Behavior;

public interface IConfigDesign : IDesign, IConfigDisplay
{
    String? GetConfigJson();
}