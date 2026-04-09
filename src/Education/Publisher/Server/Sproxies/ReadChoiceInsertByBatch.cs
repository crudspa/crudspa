namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ReadChoiceInsertByBatch
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ReadChoice readChoice)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ReadChoiceInsertByBatch";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ReadQuestionId", readChoice.ReadQuestionId);
        command.AddParameter("@Text", readChoice.Text);
        command.AddParameter("@IsCorrect", readChoice.IsCorrect ?? false);
        command.AddParameter("@ImageFileId", readChoice.ImageFileFile.Id);
        command.AddParameter("@AudioFileId", readChoice.AudioFileFile.Id);
        command.AddParameter("@Ordinal", readChoice.Ordinal);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}