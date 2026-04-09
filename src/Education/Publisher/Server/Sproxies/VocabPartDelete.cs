namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class VocabPartDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, VocabPart vocabPart)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.VocabPartDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", vocabPart.Id);

        await command.Execute(connection, transaction);
    }
}