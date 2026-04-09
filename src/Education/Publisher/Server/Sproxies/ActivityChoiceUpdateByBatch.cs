namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ActivityChoiceUpdateByBatch
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ActivityChoice activityChoice)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ActivityChoiceUpdateByBatch";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", activityChoice.Id);
        command.AddParameter("@ActivityId", activityChoice.ActivityId);
        command.AddParameter("@Text", activityChoice.Text);
        command.AddParameter("@AudioFileId", activityChoice.AudioFile?.Id);
        command.AddParameter("@ImageFileId", activityChoice.ImageFile?.Id);
        command.AddParameter("@IsCorrect", activityChoice.IsCorrect ?? false);
        command.AddParameter("@ColumnOrdinal", activityChoice.ColumnOrdinal);
        command.AddParameter("@Ordinal", activityChoice.Ordinal);

        await command.Execute(connection, transaction);
    }
}