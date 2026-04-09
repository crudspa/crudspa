namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ActivityUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Activity activity)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ActivityUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", activity.Id);
        command.AddParameter("@ActivityTypeId", activity.ActivityTypeId);
        command.AddParameter("@ContentAreaId", activity.ContentAreaId);
        command.AddParameter("@ContextText", activity.ContextText);
        command.AddParameter("@ContextAudioFileId", activity.ContextAudioFile?.Id);
        command.AddParameter("@ContextImageFileId", activity.ContextImageFile?.Id);
        command.AddParameter("@ExtraText", activity.ExtraText);

        await command.Execute(connection, transaction);
    }
}