namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ListenQuestionUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ListenQuestion listenQuestion)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ListenQuestionUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", listenQuestion.Id);
        command.AddParameter("@Text", listenQuestion.Text);
        command.AddParameter("@AudioFileId", listenQuestion.AudioFileFile.Id);
        command.AddParameter("@IsPreview", listenQuestion.IsPreview ?? false);
        command.AddParameter("@PageBreakBefore", listenQuestion.PageBreakBefore ?? false);
        command.AddParameter("@HasCorrectChoice", listenQuestion.HasCorrectChoice ?? false);
        command.AddParameter("@RequireTextInput", listenQuestion.RequireTextInput ?? false);
        command.AddParameter("@CategoryId", listenQuestion.CategoryId);
        command.AddParameter("@TypeId", listenQuestion.TypeId);
        command.AddParameter("@ImageFileId", listenQuestion.ImageFileFile.Id);

        await command.Execute(connection, transaction);
    }
}