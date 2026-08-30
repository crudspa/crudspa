namespace Crudspa.Framework.Auth.Shared.Contracts.Behavior;

public interface IAuthProvider
{
    String Key { get; }
    String ChallengeScheme { get; }
    String SessionScheme { get; }
    Boolean Enabled { get; }
}