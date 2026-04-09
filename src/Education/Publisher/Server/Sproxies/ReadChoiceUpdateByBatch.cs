namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ReadChoiceUpdateByBatch
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ReadChoice readChoice)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ReadChoiceUpdateByBatch";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", readChoice.Id);
        command.AddParameter("@ReadQuestionId", readChoice.ReadQuestionId);
        command.AddParameter("@Text", readChoice.Text);
        command.AddParameter("@IsCorrect", readChoice.IsCorrect ?? false);
        command.AddParameter("@ImageFileId", readChoice.ImageFileFile.Id);
        command.AddParameter("@AudioFileId", readChoice.AudioFileFile.Id);
        command.AddParameter("@Ordinal", readChoice.Ordinal);

        await command.Execute(connection, transaction);
    }
}