namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class LessonInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Lesson lesson)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.LessonInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@UnitId", lesson.UnitId);
        command.AddParameter("@Title", 75, lesson.Title);
        command.AddParameter("@StatusId", lesson.StatusId);
        command.AddParameter("@ImageId", lesson.ImageFile.Id);
        command.AddParameter("@GuideImageId", lesson.GuideImageFile.Id);
        command.AddParameter("@GuideText", lesson.GuideText);
        command.AddParameter("@GuideAudioId", lesson.GuideAudioFile.Id);
        command.AddParameter("@RequiresAchievementId", lesson.RequiresAchievementId);
        command.AddParameter("@RequireSequentialCompletion", lesson.RequireSequentialCompletion ?? true);
        command.AddParameter("@Treatment", lesson.Treatment ?? true);
        command.AddParameter("@Control", lesson.Control ?? true);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}