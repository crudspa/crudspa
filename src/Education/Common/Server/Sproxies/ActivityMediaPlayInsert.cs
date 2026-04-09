namespace Crudspa.Education.Common.Server.Sproxies;

public static class ActivityMediaPlayInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ActivityMediaPlay activityMediaPlay)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationCommon.ActivityMediaPlayInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@AudioFileId", activityMediaPlay.MediaPlay?.AudioFileId);
        command.AddParameter("@VideoFileId", activityMediaPlay.MediaPlay?.VideoFileId);
        command.AddParameter("@Started", activityMediaPlay.MediaPlay?.Started);
        command.AddParameter("@Canceled", activityMediaPlay.MediaPlay?.Canceled);
        command.AddParameter("@Completed", activityMediaPlay.MediaPlay?.Completed);
        command.AddParameter("@AssignmentBatchId", activityMediaPlay.AssignmentBatchId);
        command.AddParameter("@ActivityId", activityMediaPlay.ActivityId);
        command.AddParameter("@ActivityChoiceId", activityMediaPlay.ActivityChoiceId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}