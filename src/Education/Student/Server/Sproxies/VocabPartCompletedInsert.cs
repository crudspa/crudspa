namespace Crudspa.Education.Student.Server.Sproxies;

public static class VocabPartCompletedInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, VocabPartCompleted vocabPartCompleted)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.VocabPartCompletedInsert";
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@AssignmentId", vocabPartCompleted.AssignmentId);
        command.AddParameter("@VocabPartId", vocabPartCompleted.VocabPartId);
        command.AddParameter("@DeviceTimestamp", vocabPartCompleted.DeviceTimestamp);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}