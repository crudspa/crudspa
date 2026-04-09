namespace Crudspa.Education.Student.Server.Sproxies;

public static class ReadChoiceSelectionInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ReadChoiceSelection readChoiceSelection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.ReadChoiceSelectionInsert";
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@AssignmentId", readChoiceSelection.AssignmentId);
        command.AddParameter("@ChoiceId", readChoiceSelection.ChoiceId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}