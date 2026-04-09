namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ListenQuestionInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ListenQuestion listenQuestion)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ListenQuestionInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ListenPartId", listenQuestion.ListenPartId);
        command.AddParameter("@Text", listenQuestion.Text);
        command.AddParameter("@AudioFileId", listenQuestion.AudioFileFile.Id);
        command.AddParameter("@IsPreview", listenQuestion.IsPreview ?? false);
        command.AddParameter("@PageBreakBefore", listenQuestion.PageBreakBefore ?? false);
        command.AddParameter("@HasCorrectChoice", listenQuestion.HasCorrectChoice ?? false);
        command.AddParameter("@RequireTextInput", listenQuestion.RequireTextInput ?? false);
        command.AddParameter("@CategoryId", listenQuestion.CategoryId);
        command.AddParameter("@TypeId", listenQuestion.TypeId);
        command.AddParameter("@ImageFileId", listenQuestion.ImageFileFile.Id);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}