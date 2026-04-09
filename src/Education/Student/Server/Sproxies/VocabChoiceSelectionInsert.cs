namespace Crudspa.Education.Student.Server.Sproxies;

public static class VocabChoiceSelectionInsert
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, VocabChoiceSelection vocabChoiceSelection)
    {
        foreach (var choiceId in vocabChoiceSelection.Choices)
        {
            await using var command = new SqlCommand();
            command.CommandText = "EducationStudent.VocabChoiceSelectionInsert";
            command.AddParameter("@SessionId", sessionId);
            command.AddParameter("@AssignmentId", vocabChoiceSelection.AssignmentId);
            command.AddParameter("@ChoiceId", choiceId);

            await command.Execute(connection, transaction);
        }
    }
}