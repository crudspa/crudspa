using Crudspa.Education.Common.Shared.Contracts.Config.ElementType;

namespace Crudspa.Education.Common.Server.Sproxies;

public static class ActivityElementUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ActivityElement activityElement)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationCommon.ActivityElementUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", activityElement.Id);
        command.AddParameter("@ElementId", activityElement.ElementId);
        command.AddParameter("@ActivityId", activityElement.ActivityId);

        await command.Execute(connection, transaction);
    }
}