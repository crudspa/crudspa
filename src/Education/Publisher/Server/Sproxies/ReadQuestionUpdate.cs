namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ReadQuestionUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ReadQuestion readQuestion)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ReadQuestionUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", readQuestion.Id);
        command.AddParameter("@Text", readQuestion.Text);
        command.AddParameter("@AudioFileId", readQuestion.AudioFileFile.Id);
        command.AddParameter("@IsPreview", readQuestion.IsPreview ?? false);
        command.AddParameter("@PageBreakBefore", readQuestion.PageBreakBefore ?? false);
        command.AddParameter("@HasCorrectChoice", readQuestion.HasCorrectChoice ?? false);
        command.AddParameter("@RequireTextInput", readQuestion.RequireTextInput ?? false);
        command.AddParameter("@CategoryId", readQuestion.CategoryId);
        command.AddParameter("@TypeId", readQuestion.TypeId);
        command.AddParameter("@ImageFileId", readQuestion.ImageFileFile.Id);

        await command.Execute(connection, transaction);
    }
}