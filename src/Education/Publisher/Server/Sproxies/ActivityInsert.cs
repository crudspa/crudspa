namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ActivityInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Activity activity)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ActivityInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ActivityTypeId", activity.ActivityTypeId);
        command.AddParameter("@ContentAreaId", activity.ContentAreaId);
        command.AddParameter("@ContextText", activity.ContextText);
        command.AddParameter("@ContextAudioFileId", activity.ContextAudioFile.Id);
        command.AddParameter("@ContextImageFileId", activity.ContextImageFile.Id);
        command.AddParameter("@ExtraText", activity.ExtraText);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}