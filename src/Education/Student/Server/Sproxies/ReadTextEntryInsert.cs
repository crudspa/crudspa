namespace Crudspa.Education.Student.Server.Sproxies;

public static class ReadTextEntryInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ReadTextEntry readTextEntry)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.ReadTextEntryInsert";
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@AssignmentId", readTextEntry.AssignmentId);
        command.AddParameter("@QuestionId", readTextEntry.QuestionId);
        command.AddParameter("@Text", readTextEntry.Text);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}