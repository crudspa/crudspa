namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ReadQuestionDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ReadQuestion readQuestion)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ReadQuestionDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", readQuestion.Id);

        await command.Execute(connection, transaction);
    }
}