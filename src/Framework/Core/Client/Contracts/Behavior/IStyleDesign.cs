namespace Crudspa.Framework.Core.Client.Contracts.Behavior;

public interface IStyleDesign : IConfigDesign, IValidates
{
    Guid? ContentPortalId { get; set; }
}