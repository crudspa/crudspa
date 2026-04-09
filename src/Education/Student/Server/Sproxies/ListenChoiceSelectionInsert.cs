namespace Crudspa.Education.Student.Server.Sproxies;

public static class ListenChoiceSelectionInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ListenChoiceSelection listenChoiceSelection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.ListenChoiceSelectionInsert";
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@AssignmentId", listenChoiceSelection.AssignmentId);
        command.AddParameter("@ChoiceId", listenChoiceSelection.ChoiceId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}