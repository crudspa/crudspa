namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ListenChoiceInsertByBatch
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ListenChoice listenChoice)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ListenChoiceInsertByBatch";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ListenQuestionId", listenChoice.ListenQuestionId);
        command.AddParameter("@Text", listenChoice.Text);
        command.AddParameter("@IsCorrect", listenChoice.IsCorrect ?? false);
        command.AddParameter("@ImageFileId", listenChoice.ImageFileFile.Id);
        command.AddParameter("@AudioFileId", listenChoice.AudioFileFile.Id);
        command.AddParameter("@Ordinal", listenChoice.Ordinal);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}