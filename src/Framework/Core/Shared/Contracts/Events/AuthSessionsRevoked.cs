namespace Crudspa.Framework.Core.Shared.Contracts.Events;

public class AuthSessionsRevoked
{
    public IList<Guid> PolicyIds { get; set; } = [];
}