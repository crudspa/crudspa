using Crudspa.Framework.Auth.Shared.Contracts.Data;
using Microsoft.Data.SqlClient;

namespace Crudspa.Framework.Auth.Server.Contracts.Behavior;

public interface IAuthConfigRepository
{
    Task<AuthConfig> Select(String connection, Guid? organizationId);
    Task<IList<Guid>> Save(SqlConnection connection, SqlTransaction transaction, Guid? sessionId, Guid? organizationId, AuthConfig config);
    Task PublishRevocations(IList<Guid> policyIds);
}