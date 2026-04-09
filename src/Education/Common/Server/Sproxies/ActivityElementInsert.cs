using Crudspa.Education.Common.Shared.Contracts.Config.ElementType;

namespace Crudspa.Education.Common.Server.Sproxies;

public static class ActivityElementInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ActivityElement activityElement)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationCommon.ActivityElementInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ElementId", activityElement.ElementId);
        command.AddParameter("@ActivityId", activityElement.ActivityId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}