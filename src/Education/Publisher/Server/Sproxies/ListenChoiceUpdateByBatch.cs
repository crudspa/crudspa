namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ListenChoiceUpdateByBatch
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ListenChoice listenChoice)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ListenChoiceUpdateByBatch";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", listenChoice.Id);
        command.AddParameter("@ListenQuestionId", listenChoice.ListenQuestionId);
        command.AddParameter("@Text", listenChoice.Text);
        command.AddParameter("@IsCorrect", listenChoice.IsCorrect ?? false);
        command.AddParameter("@ImageFileId", listenChoice.ImageFileFile.Id);
        command.AddParameter("@AudioFileId", listenChoice.AudioFileFile.Id);
        command.AddParameter("@Ordinal", listenChoice.Ordinal);

        await command.Execute(connection, transaction);
    }
}