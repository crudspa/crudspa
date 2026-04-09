namespace Crudspa.Framework.Core.Client.Contracts.Behavior;

public interface IConfigDisplay : IDisplay
{
    String? ConfigJson { get; set; }
}