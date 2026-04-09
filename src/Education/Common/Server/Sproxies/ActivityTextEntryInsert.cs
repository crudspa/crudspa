namespace Crudspa.Education.Common.Server.Sproxies;

public static class ActivityTextEntryInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ActivityTextEntry activityTextEntry)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationCommon.ActivityTextEntryInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@AssignmentId", activityTextEntry.AssignmentId);
        command.AddParameter("@Text", activityTextEntry.Text);
        command.AddParameter("@Made", activityTextEntry.Made);
        command.AddParameter("@Ordinal", activityTextEntry.Ordinal);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}