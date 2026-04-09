namespace Crudspa.Education.School.Server.Contracts.Behavior;

public interface ISecretCodeService
{
    Task<String> Generate();
}