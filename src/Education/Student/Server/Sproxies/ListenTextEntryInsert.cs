namespace Crudspa.Education.Student.Server.Sproxies;

public static class ListenTextEntryInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ListenTextEntry listenTextEntry)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.ListenTextEntryInsert";
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@AssignmentId", listenTextEntry.AssignmentId);
        command.AddParameter("@QuestionId", listenTextEntry.QuestionId);
        command.AddParameter("@Text", listenTextEntry.Text);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}