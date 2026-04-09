namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ListenQuestionDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ListenQuestion listenQuestion)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ListenQuestionDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", listenQuestion.Id);

        await command.Execute(connection, transaction);
    }
}