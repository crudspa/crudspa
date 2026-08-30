using Crudspa.Education.Rostering.Shared.Contracts.Data;
using Crudspa.Framework.Core.Server.Extensions;
using Microsoft.Data.SqlClient;

namespace Crudspa.Education.Rostering.Server.Sproxies;

public static class RosterSourceSelect
{
    public static async Task<RosterSource?> Execute(
        SqlConnection connection,
        SqlTransaction? transaction,
        Guid id)
    {
        await using var command = new SqlCommand { CommandText = "EducationRostering.RosterSourceSelect" };
        command.AddParameter("@Id", id);
        return await command.ReadSingle(connection, transaction, RosterSourceSelectForOrganization.Read);
    }
}