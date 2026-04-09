namespace Crudspa.Education.School.Server.Sproxies;

public static class ClassRecordingDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ClassRecording classRecording)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.ClassRecordingDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", classRecording.Id);

        await command.Execute(connection, transaction);
    }
}