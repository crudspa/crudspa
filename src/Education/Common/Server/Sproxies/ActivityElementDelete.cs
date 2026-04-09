using Crudspa.Education.Common.Shared.Contracts.Config.ElementType;

namespace Crudspa.Education.Common.Server.Sproxies;

public static class ActivityElementDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ActivityElement activityElement)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationCommon.ActivityElementDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", activityElement.Id);

        await command.Execute(connection, transaction);
    }
}